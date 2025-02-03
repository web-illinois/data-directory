using System.Text;
using OpenSearch.Net;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_external.DataWarehouse;
using uofi_itp_directory_external.Experts;
using uofi_itp_directory_external.ImageManager;
using uofi_itp_directory_external.ProgramCourse;

namespace uofi_itp_directory_search.LoadHelper {

    public class LoadManager(DataWarehouseManager? dataWarehouseManager, EmployeeHelper? employeeHelper, ProgramCourseInformation? programCourseInformation, IllinoisExpertsManager? illinoisExpertsManager, AreaHelper? areaHelper, string? searchUrl, OpenSearchLowLevelClient? openSearchLowLevelClient) {
        private readonly AreaHelper _areaHelper = areaHelper ?? default!;
        private readonly DataWarehouseManager _dataWarehouseManager = dataWarehouseManager ?? default!;
        private readonly EmployeeHelper _employeeHelper = employeeHelper ?? default!;
        private readonly IllinoisExpertsManager _illinoisExpertsManager = illinoisExpertsManager ?? default!;
        private readonly StringBuilder _logger = new();
        private readonly ProgramCourseInformation _programCourseInformation = programCourseInformation ?? default!;
        private readonly string _searchUrl = searchUrl ?? "";
        private readonly OpenSearchLowLevelClient _openSearchLowLevelClient = openSearchLowLevelClient ?? default!;

        public async Task<string> LoadPerson(string netId, string source) {
            AddLog($"Starting process with Net ID {netId} and source {source}");
            try {
                var settings = await _areaHelper.GetAreaSettingsBySource(source);
                if (settings == null) {
                    AddLog($"Source {source} not found.");
                    return _logger.ToString();
                }
                if (settings != null && settings.UrlPeopleRefreshType != PeopleRefreshTypeEnum.Default) {
                    AddLog($"Source {source} not listed as default refresh.");
                    return _logger.ToString();
                }
                //TODO add more area parameters here, potentially split out from the LoadPerson
                var useCampusPictures = false;
                var personSetter = new PersonSetter(_searchUrl, _openSearchLowLevelClient, AddLog);
                AddLog("Getting initial person information from EDW. ");
                var edwItem = await _dataWarehouseManager.GetDataWarehouseItem(netId);
                if (edwItem == null || !edwItem.IsValid) {
                    AddLog($"Username {netId} could not be found in EDW - removing user from Amazon OpenSearch Service");
                    if (await personSetter.DeleteSingle(source, netId))
                        AddLog($"Username {netId} removed from source {source}");
                    return _logger.ToString();
                }
                AddLog("Getting information from IT Partners Directory Application");
                var employee = await _employeeHelper.GetEmployeeReadOnly(netId, source);
                if (employee == null || employee.JobProfiles.Count == 0) {
                    AddLog($"Username {netId} does not have any appointments for source {source} - removing user from Amazon OpenSearch Service");
                    if (await personSetter.DeleteSingle(source, netId))
                        AddLog($"Username {netId} removed from source {source}");
                    return _logger.ToString();
                }
                AddLog("Getting Experts information");
                var expertsProfile = await _illinoisExpertsManager.GetExperts(netId);
                AddLog(useCampusPictures ? "Getting image from campus" : "Validating image");
                var imageUrl = useCampusPictures ? DirectoryImage.GetCampusImagePathFromNetId(netId) : DirectoryImage.CheckImage(employee.PhotoUrl);
                AddLog("Getting courses from programcourses.itpartners.illinois.edu");
                var courses = _programCourseInformation.GetCourses(source, netId, edwItem.Uin).ToList();
                AddLog($"Combining information: EDW, IT Partners Directory, Image{(expertsProfile.UseExperts ? ", Experts" : "")}{(courses.Count > 0 ? ", Courses" : "")}");
                var profile = EmployeeTranslator.Translate(edwItem, employee, imageUrl, courses, expertsProfile, source);
                AddLog("Adding to directory using " + source);
                if (await personSetter.SaveSingle(profile)) {
                    AddLog($"Username {netId} updated in source {source}");
                    AddLog("Completed Process");
                }
            } catch (Exception e) {
                AddLog($"Error in process, aborting: {e}");
            }
            return _logger.ToString();
        }

        private void AddLog(string message) => _logger.AppendLine($"{message} ({DateTime.Now.ToShortTimeString()}). ");
    }
}