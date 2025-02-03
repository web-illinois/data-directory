using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.Helpers {

    public class EmployeeAreaHelper(DirectoryRepository directoryRepository) {
        private readonly DirectoryRepository _directoryRepository = directoryRepository;

        public static string ConvertName(string name) => Regex.Replace(name, "[^A-Za-z0-9 ]", "").Replace(" ", "-").ToLowerInvariant();

        public static string ConvertProfileUrl(string profileUrl, string netid, string name) => profileUrl.Replace("{netid}", ConvertNetId(netid)).Replace("{name}", ConvertName(name));

        public async Task<string> ActivitiesInstructions(string netid) => (await GetSettings(netid)).areaSettings.InstructionsEmployeeActivities;

        public async Task<string> CvInstructions(string netid) => (await GetSettings(netid)).areaSettings.InstructionsEmployeeCv;

        public async Task<string> EmployeeInstructions(string netid) => (await GetSettings(netid)).areaSettings.InstructionsEmployee;

        public async Task<string> HeadshotInstructions(string netid) => (await GetSettings(netid)).areaSettings.InstructionsEmployeeHeadshot;

        public async Task<string> OfficeInstructions(int? officeId) => (await GetSettings(officeId)).InstructionsOffice;

        public async Task<string> ProfileViewUrl(string netid, string newFullName) {
            var (areaSettings, _) = await GetSettings(netid);
            return ConvertProfileUrl(areaSettings.UrlProfile, netid, newFullName);
        }

        public async Task<string> ProfileViewUrlReadOnly(string netid) {
            var (areaSettings, fullName) = await GetSettings(netid);
            return ConvertProfileUrl(areaSettings.UrlProfile, netid, fullName);
        }

        public async Task<bool> ShouldUseExperts(string netid) => (await GetSettings(netid)).areaSettings.AllowInformationForIllinoisExpertsMembers;

        public async Task<string> SignatureInstructions(string netid) => (await GetSettings(netid)).areaSettings.InstructionsEmployeeSignature;

        private static string ConvertNetId(string netid) => netid.Replace("@illinois.edu", "").ToLowerInvariant();

        private async Task<(AreaSettings areaSettings, string fullName)> GetSettings(string netid) {
            var employee = await _directoryRepository.ReadAsync(d => d.Employees.Include(e => e.JobProfiles).ThenInclude(jp => jp.Office).SingleOrDefault(e => e.NetId == netid));
            return employee == null || employee.Id == 0 ? (new AreaSettings(), "") : (await _directoryRepository.ReadAsync(d => d.AreaSettings.Single(a => a.AreaId == employee.PrimaryJobProfile.Office.AreaId)), employee.Name);
        }

        private async Task<AreaSettings> GetSettings(int? officeId) {
            var office = await _directoryRepository.ReadAsync(d => d.Offices.SingleOrDefault(o => o.Id == officeId));
            return office == null || office.Id == 0 || office.AreaId == 0 ? new AreaSettings() : await _directoryRepository.ReadAsync(d => d.AreaSettings.Single(a => a.AreaId == office.AreaId));
        }
    }
}