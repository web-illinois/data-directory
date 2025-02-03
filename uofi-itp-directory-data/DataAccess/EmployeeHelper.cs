using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Helpers;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory_data.DataAccess {

    public class EmployeeHelper(DirectoryRepository? directoryRepository, DirectoryHookHelper? directoryHookHelper, DirectoryContext? directoryContext, EmployeeAreaHelper? employeeAreaHelper, LogHelper? logHelper) {
        private readonly DirectoryContext _directoryContext = directoryContext ?? throw new ArgumentNullException("directoryContext");
        private readonly DirectoryHookHelper? _directoryHookHelper = directoryHookHelper;
        private readonly DirectoryRepository _directoryRepository = directoryRepository ?? throw new ArgumentNullException("directoryRepository");
        private readonly EmployeeAreaHelper _employeeAreaHelper = employeeAreaHelper ?? throw new ArgumentNullException("employeeAreaHelper");
        private readonly LogHelper? _logHelper = logHelper;

        public async Task<int> DeleteEmployee(string netId) {
            var returnValue = 0;
            var employeeId = (await _directoryRepository.ReadAsync(d => d.Employees.FirstOrDefault(e => e.NetId == netId)))?.Id ?? 0;
            if (employeeId != 0) {
                _ = await _directoryContext.EmployeeActivities.Where(e => e.EmployeeId == employeeId).ExecuteDeleteAsync();
                _ = await _directoryContext.JobProfiles.Where(e => e.EmployeeProfileId == employeeId).ExecuteDeleteAsync();
                _ = await _directoryContext.EmployeeHours.Where(e => e.EmployeeId == employeeId).ExecuteDeleteAsync();
                returnValue = await _directoryContext.Employees.Where(e => e.Id == employeeId).ExecuteDeleteAsync();
                _ = await _directoryRepository.CreateAsync(new Log { IsActive = true, SubjectType = LogTypeEnum.Employee, SubjectId = employeeId, SubjectText = netId, ChangedByNetId = "automated", ChangeType = "Deleted", Data = "NetID is removed from EDW" });
            }
            return returnValue;
        }

        public async Task<Employee?> GetEmployee(int? id, string name) {
            var employee = await _directoryRepository.ReadAsync(d => d.Employees.Include(e => e.JobProfiles).ThenInclude(jp => jp.Tags).Include(e => e.JobProfiles).ThenInclude(jp => jp.Office).Include(e => e.EmployeeActivities).Include(e => e.EmployeeHours).FirstOrDefault(e => e.NetId == name && id == null || e.Id == id));
            if (employee == null) {
                return null;
            }
            if (employee.NetId == name) {
                employee.IsEntryDisabled = false;
                employee.IsCurrentUser = true;
            }
            var officesEmployeeIsIn = employee.JobProfiles.Select(jp => jp.OfficeId).ToList();

            var securityEntriesUserIsIn = (await _directoryRepository.ReadAsync(d => d.SecurityEntries.Where(se => se.Email == name && se.IsActive))).ToList();

            // start: if a security entry has an area, include all the offices in the area
            var areaIds = securityEntriesUserIsIn.Where(se => se.AreaId != null).Select(se => se.AreaId);

            var officeIdInAreas = areaIds.Any() ? [.. (await _directoryRepository.ReadAsync(d => d.Offices.Where(o => areaIds.Contains(o.AreaId)).Select(o => o.Id)))] : new List<int>();

            var allowFullEdit = officeIdInAreas.Any(officesEmployeeIsIn.Contains);
            // end: if a security entry has an area, include all the offices in the area

            if (securityEntriesUserIsIn.Any(se => se.IsFullAdmin || allowFullEdit || officesEmployeeIsIn.Contains(se.OfficeId ?? -1) && se.CanEditAllPeopleInUnit)) {
                employee.IsEntryDisabled = false;
            }

            foreach (var profile in employee.JobProfiles) {
                profile.IsEntryDisabled = !(securityEntriesUserIsIn.Any(se => se.IsFullAdmin) || officeIdInAreas.Contains(profile.OfficeId) || securityEntriesUserIsIn.Select(se => se.OfficeId ?? -1).Contains(profile.OfficeId));
            }

            return employee;
        }

        public async Task<Employee?> GetEmployeeForSignature(int? id, string name) => await _directoryRepository.ReadAsync(d => d.Employees.Include(e => e.JobProfiles).ThenInclude(jp => jp.Office).ThenInclude(o => o.Area).FirstOrDefault(e => e.NetId == name && id == null || e.Id == id));

        public async Task<Employee?> GetEmployeeReadOnly(string netId, string source) {
            netId = netId.Replace("@illinois.edu", "") + "@illinois.edu";
            var employee = await _directoryRepository.ReadAsync(d => d.Employees.Include(e => e.JobProfiles).ThenInclude(jp => jp.Office).ThenInclude(o => o.Area).ThenInclude(a => a.AreaSettings).Include(e => e.JobProfiles).ThenInclude(jp => jp.Tags).Include(e => e.EmployeeActivities).Include(e => e.EmployeeHours).FirstOrDefault(e => e.NetId == netId));
            if (employee != null && employee.JobProfiles != null) {
                employee.JobProfiles = employee.JobProfiles.Where(j => j.Office.Area.AreaSettings.InternalCode == source && j.Office.CanAddPeople).ToList();
            }
            return employee;
        }

        public async Task<AreaSettings> GetEmployeeSettings(Employee? employee) => employee == null ?
            new AreaSettings() :
            await _directoryRepository.ReadAsync(d => d.Offices.Include(o => o.Area).ThenInclude(a => a.AreaSettings).SingleOrDefault(o => o.Id == employee.PrimaryJobProfile.OfficeId)?.Area?.AreaSettings) ?? new AreaSettings();

        public async Task<int> RemoveTag(JobProfileTag? tag) => await _directoryRepository.DeleteAsync(tag);

        public async Task<int> SaveEmployee(Employee employee, string changedByNetId, string message) {
            employee.ProfileUrl = await _employeeAreaHelper.ProfileViewUrl(employee.NetId, employee.Name);
            var returnValue = await _directoryRepository.UpdateAsync(employee);
            if (_directoryHookHelper != null) {
                _ = await _directoryHookHelper.SendHook(employee.Id, true);
            }
            if (_logHelper != null) {
                _ = await _logHelper.CreateEmployeeLog(changedByNetId, message, employee.ToString(), employee.Id, employee.NetId);
            }
            return returnValue;
        }

        // Used only if the user changed the URL for the area -- very slow.
        public async Task<int> UpdateAllEmployeeUrlProfiles(int areaId, string url) {
            var returnValue = 0;
            foreach (var employee in await _directoryRepository.ReadAsync(d => d.Employees.Include(e => e.JobProfiles).ThenInclude(jp => jp.Office).Where(e => e.JobProfiles.Any(jp => jp.Office.AreaId == areaId)))) {
                if (employee.PrimaryJobProfile.Office.AreaId == areaId) {
                    employee.ProfileUrl = EmployeeAreaHelper.ConvertProfileUrl(url, employee.NetId, employee.Name);
                    returnValue += await _directoryRepository.UpdateAsync(employee);
                }
            }
            return returnValue;
        }
    }
}