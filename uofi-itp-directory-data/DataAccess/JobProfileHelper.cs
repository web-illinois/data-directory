using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_external.DataWarehouse;

namespace uofi_itp_directory_data.DataAccess {

    public class JobProfileHelper(DirectoryRepository directoryRepository, DataWarehouseManager dataWarehouseManager, DirectoryHookHelper directoryHookHelper, LogHelper logHelper) {
        private readonly DataWarehouseManager _dataWarehouseManager = dataWarehouseManager;
        private readonly DirectoryHookHelper _directoryHookHelper = directoryHookHelper;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;
        private readonly LogHelper _logHelper = logHelper;

        public async Task<(int employeeId, string message)> GenerateJobProfile(int officeId, string netid, string changedByNetId, string title = "", ProfileCategoryTypeEnum profileCategoryTypeEnum = ProfileCategoryTypeEnum.None) {
            netid = netid.Trim().ToLowerInvariant();
            if (!netid.EndsWith("@illinois.edu"))
                netid += "@illinois.edu";

            var checkExistingProfile = await _directoryRepository.ReadAsync(d => d.JobProfiles.Include(jp => jp.EmployeeProfile).Any(jp => jp.OfficeId == officeId && jp.EmployeeProfile.NetId == netid));
            if (checkExistingProfile)
                return (0, $"Profile already exists for {netid}");
            var name = await _dataWarehouseManager.GetDataWarehouseItem(netid);
            if (!name.IsValid)
                return (0, $"Net ID '{netid}' not found - this may be because they have their information suppressed by campus.");
            var existingEmployee = await _directoryRepository.ReadAsync(d => d.Employees.FirstOrDefault(e => e.NetId == netid));

            if (existingEmployee == null) {
                var employee = new Employee {
                    NetId = netid,
                    ListedNameFirst = name.FirstName,
                    ListedNameLast = name.LastName,
                    IsActive = true,
                    LastUpdated = DateTime.Now,
                    EmployeeHours = [new() { Day = DayOfWeek.Sunday }, new() { Day = DayOfWeek.Monday }, new() { Day = DayOfWeek.Tuesday }, new() { Day = DayOfWeek.Wednesday }, new() { Day = DayOfWeek.Thursday }, new() { Day = DayOfWeek.Friday }, new() { Day = DayOfWeek.Saturday }],
                    JobProfiles = [new() { IsActive = true, InternalOrder = 3, LastUpdated = DateTime.Now, Title = !string.IsNullOrWhiteSpace(title) ? title : name.Title, OfficeId = officeId, Category = profileCategoryTypeEnum }]
                };
                _ = await _directoryRepository.CreateAsync(employee);
                _ = await _logHelper.CreateEmployeeLog(changedByNetId, "Added New Employee", "", employee.Id, employee.NetId);
                _ = await _logHelper.CreateProfileLog(changedByNetId, "Added Profile to New Employee", "", employee.Id, employee.NetId);
                return (employee.Id, $"Employee {name.Name} created with new information");
            }

            _ = await _directoryRepository.CreateAsync(new JobProfile { IsActive = true, InternalOrder = 3, LastUpdated = DateTime.Now, Title = !string.IsNullOrWhiteSpace(title) ? title : name.Title, EmployeeProfileId = existingEmployee.Id, OfficeId = officeId, Category = profileCategoryTypeEnum });
            _ = await _logHelper.CreateProfileLog(changedByNetId, "Added Profile to Existing Employee", "", existingEmployee.Id, existingEmployee.NetId);

            return (existingEmployee.Id, $"Employee {name.Name} created with existing information");
        }

        public async Task<List<JobProfileThinObject>> GetJobProfileThinObjects(int officeId) =>
            [.. await _directoryRepository.ReadAsync(d => d.JobProfiles.Include(jp => jp.EmployeeProfile).Where(jp => jp.OfficeId == officeId).Select(jp => new JobProfileThinObject { Display = jp.EmployeeProfile.Name + " (" + jp.EmployeeProfile.NetId + ")", EmployeeId = jp.EmployeeProfileId, EmployeeNetId = jp.EmployeeProfile.NetId, JobProfileId = jp.Id }).ToList().OrderBy(to => to.Display))];

        public async Task<int> RemoveJobProfile(int jobProfileId, int employeeId, string employeeNetId, string changedByNetId) {
            var job = _directoryRepository.Read(d => d.JobProfiles.Single(jp => jp.Id == jobProfileId));
            var officeId = job.OfficeId;
            var returnValue = await _directoryRepository.DeleteAsync(job);
            _ = await _logHelper.CreateProfileLog(changedByNetId, "Removed Profile", job.ToString(), employeeId, employeeNetId);
            _ = await _directoryHookHelper.SendHookToRemoveEmployee(employeeId, employeeNetId, officeId);
            return returnValue;
        }
    }
}