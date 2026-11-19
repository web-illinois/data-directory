using System.Text.Json.Nodes;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.WebTools {
    public class WebtoolsImporter(DirectoryRepository? directoryRepository, OfficeHelper officeHelper, JobProfileHelper jobProfileHelper) {
        private readonly DirectoryRepository _directoryRepository = directoryRepository ?? throw new ArgumentNullException("directoryRepository");
        private readonly OfficeHelper _officeHelper = officeHelper ?? throw new ArgumentNullException("officeHelper");
        private readonly JobProfileHelper _jobProfileHelper = jobProfileHelper ?? throw new ArgumentNullException("jobProfileHelper");

        public async Task<string> ImportDepartment(Office office, string netId) => await ImportDepartment(office.DepartmentId, office.Id, netId, true);

        public async Task<string> ImportDepartment(string departmentKey, int officeId, string netId, bool importPeople) {
            if (string.IsNullOrWhiteSpace(departmentKey)) {
                return "Department key is required";
            }
            var existingToolkitDepartment = await _directoryRepository.ReadAsync<ToolkitDepartment>(x => x.ToolkitDepartments.FirstOrDefault(t => t.Key == departmentKey));
            if (existingToolkitDepartment == null) {
                return $"Could not find ToolkitDepartment with key {departmentKey}";
            }
            var office = await _officeHelper.GetOfficeById(officeId, netId);
            var root = JsonNode.Parse(existingToolkitDepartment.Json);
            office.DepartmentId = departmentKey;
            office.Title = root?["name"]?.ToString() ?? office.Title;
            office.Address = root?["address2"]?.ToString() ?? office.Address;
            office.Building = root?["address1"]?.ToString() ?? office.Building;
            office.City = root?["city"]?.ToString() ?? office.City;
            office.State = root?["state"]?.ToString() ?? office.State;
            office.BuildingCode = root?["buildingcode"]?.ToString() ?? office.BuildingCode;
            office.ZipCode = root?["zipcode"]?.ToString() ?? office.ZipCode;
            office.ExternalUrl = root?["url"]?.ToString() ?? office.ExternalUrl;
            if (root?["categories"] is JsonArray items) {
                foreach (var item in items) {
                    if (item != null && item?["name"]?.ToString() == "Full Directory Listing") {
                        office.WebToolsLinks = item?["value"]?.ToString() ?? "";
                    }
                }
            }
            office.WebToolsListing = root?["links"]?.ToString() ?? office.WebToolsListing;
            var listOfNetIds = new List<string>();
            var currentJobProfiles = (await _jobProfileHelper.GetJobProfileThinObjects(officeId)) ?? [];
            if (importPeople && root?["people_groups"] is JsonArray people_groups) {
                foreach (var people_group in people_groups) {
                    var name = people_group?["name"]?.ToString() ?? "";
                    var profileCategory = ProfileCategoryTypeEnum.Staff;
                    if (name == "Faculty") {
                        profileCategory = ProfileCategoryTypeEnum.Faculty;
                    } else if (name == "Student") {
                        profileCategory = ProfileCategoryTypeEnum.Graduate_Student;
                    }
                    if (people_group?["members"] is JsonArray members) {
                        foreach (var member in members) {
                            _ = await _jobProfileHelper.GenerateJobProfile(officeId, member?["user_id"]?.ToString() ?? "", netId, member?["role1"]?.ToString() ?? "", profileCategory);
                            listOfNetIds.Add(member?["user_id"]?.ToString() ?? "");
                        }
                    }
                }
                foreach (var jobProfile in currentJobProfiles.Where(c => !listOfNetIds.Contains(c.EmployeeNetId))) {
                    _ = await _jobProfileHelper.RemoveJobProfile(jobProfile.JobProfileId, jobProfile.EmployeeId, jobProfile.EmployeeNetId, "webtools");
                }
            }
            _ = await _officeHelper.UpdateOffice(office, netId);
            return $"Added {departmentKey} to {office.Title} with number of people: {listOfNetIds.Count()}";
        }

        public async Task<string> ImportService(Office office, string netId) => await ImportService(office.DepartmentId, office.Id, netId, true);

        public async Task<string> ImportService(string serviceKey, int officeId, string netId, bool importPeople) {
            if (string.IsNullOrWhiteSpace(serviceKey)) {
                return "Service key is required";
            }
            var existingToolkitDepartment = await _directoryRepository.ReadAsync<ToolkitService>(x => x.ToolkitServices.FirstOrDefault(t => t.Key == serviceKey));
            if (existingToolkitDepartment == null) {
                return $"Could not find ToolkitService with key {serviceKey}";
            }
            var office = await _officeHelper.GetOfficeById(officeId, netId);
            var root = JsonNode.Parse(existingToolkitDepartment.Json);
            office.ServiceId = serviceKey;
            office.Title = root?["name"]?.ToString() ?? office.Title;
            office.Address = root?["address2"]?.ToString() ?? office.Address;
            office.Building = root?["address1"]?.ToString() ?? office.Building;
            office.City = root?["city"]?.ToString() ?? office.City;
            office.State = root?["state"]?.ToString() ?? office.State;
            office.BuildingCode = root?["buildingcode"]?.ToString() ?? office.BuildingCode;
            office.ZipCode = root?["zipcode"]?.ToString() ?? office.ZipCode;
            office.ExternalUrl = root?["url"]?.ToString() ?? office.ExternalUrl;
            if (root?["categories"] is JsonArray items) {
                foreach (var item in items) {
                    if (item != null && item?["name"]?.ToString() == "About") {
                        office.WebToolsLinks = item?["value"]?.ToString() ?? "";
                    }
                }
            }
            office.WebToolsListing = root?["links"]?.ToString() ?? office.WebToolsListing;
            var numberItems = 0;
            if (importPeople && root?["people_groups"] is JsonArray people_groups) {
                _ = await _jobProfileHelper.RemoveAllJobProfilesFromOffice(officeId);
                foreach (var people_group in people_groups) {
                    var name = people_group?["name"]?.ToString() ?? "";
                    var profileCategory = ProfileCategoryTypeEnum.Staff;
                    if (name == "Faculty") {
                        profileCategory = ProfileCategoryTypeEnum.Faculty;
                    } else if (name == "Student") {
                        profileCategory = ProfileCategoryTypeEnum.Graduate_Student;
                    }

                    if (people_group?["members"] is JsonArray members) {
                        foreach (var member in members) {
                            _ = await _jobProfileHelper.GenerateJobProfile(officeId, member?["user_id"]?.ToString() ?? "", netId, member?["role1"]?.ToString() ?? "", profileCategory);
                            numberItems++;
                        }
                    }
                }
            }
            _ = await _officeHelper.UpdateOffice(office, netId);
            return $"Added service with number of people: {numberItems}";
        }

    }
}
