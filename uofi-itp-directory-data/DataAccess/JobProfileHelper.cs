﻿using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.CampusService;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory_data.DataAccess {

    public class JobProfileHelper(DirectoryRepository directoryRepository, DataWarehouseManager dataWarehouseManager, DirectoryHookHelper directoryHookHelper, LogHelper logHelper) {
        private readonly DataWarehouseManager _dataWarehouseManager = dataWarehouseManager;
        private readonly DirectoryHookHelper _directoryHookHelper = directoryHookHelper;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;
        private readonly LogHelper _logHelper = logHelper;

        public async Task<(int employeeId, string message)> GenerateJobProfile(int officeId, string netid, string changedByNetId) {
            if (!netid.EndsWith("@illinois.edu"))
                netid = netid + "@illinois.edu";

            var checkExistingProfile = await _directoryRepository.ReadAsync(d => d.JobProfiles.Include(jp => jp.EmployeeProfile).Any(jp => jp.OfficeId == officeId && jp.EmployeeProfile.NetId == netid));
            if (checkExistingProfile)
                return (0, $"Profile already exists for {netid}");
            var name = await _dataWarehouseManager.GetDataWarehouseItem(netid);
            if (!name.IsValid)
                return (0, $"Net ID '{netid}' not found");
            var existingEmployee = await _directoryRepository.ReadAsync(d => d.Employees.FirstOrDefault(e => e.NetId == netid));

            if (existingEmployee == null) {
                var employee = new Employee {
                    NetId = netid,
                    ListedNameFirst = name.FirstName,
                    ListedNameLast = name.LastName,
                    IsActive = true,
                    LastUpdated = DateTime.Now,
                    EmployeeHours = new List<EmployeeHour> { new() { Day = DayOfWeek.Sunday }, new() { Day = DayOfWeek.Monday }, new() { Day = DayOfWeek.Tuesday }, new() { Day = DayOfWeek.Wednesday }, new() { Day = DayOfWeek.Thursday }, new() { Day = DayOfWeek.Friday }, new() { Day = DayOfWeek.Saturday } },
                    JobProfiles = new List<JobProfile> { new() { IsActive = false, InternalOrder = 2, LastUpdated = DateTime.Now, Title = name.Title, OfficeId = officeId } }
                };
                _ = await _directoryRepository.CreateAsync(employee);
                _ = await _logHelper.CreateEmployeeLog(changedByNetId, "Added New Employee", "", employee.Id, employee.NetId);
                _ = await _logHelper.CreateProfileLog(changedByNetId, "Added Profile to New Employee", "", employee.Id, employee.NetId);
                _ = await _directoryHookHelper.SendHook(employee.Id);
                return (employee.Id, $"Employee {name.Name} created with new information");
            }

            _ = await _directoryRepository.CreateAsync(new JobProfile { IsActive = false, InternalOrder = 2, LastUpdated = DateTime.Now, Title = name.Title, EmployeeProfileId = existingEmployee.Id, OfficeId = officeId });
            _ = await _logHelper.CreateProfileLog(changedByNetId, "Added Profile to Existing Employee", "", existingEmployee.Id, existingEmployee.NetId);
            _ = await _directoryHookHelper.SendHook(existingEmployee.Id);

            return (existingEmployee.Id, $"Employee {name.Name} created with existing information");
        }

        public async Task<List<JobProfileThinObject>> GetJobProfileThinObjects(int officeId) =>
            [.. await _directoryRepository.ReadAsync(d => d.JobProfiles.Include(jp => jp.EmployeeProfile).Where(jp => jp.OfficeId == officeId).Select(jp => new JobProfileThinObject { Display = jp.EmployeeProfile.Name + " (" + jp.EmployeeProfile.NetId + ")", EmployeeId = jp.EmployeeProfileId, EmployeeNetId = jp.EmployeeProfile.NetId, JobProfileId = jp.Id }).ToList().OrderBy(to => to.Display))];

        public async Task<int> RemoveJobProfile(int jobProfileId, int employeeId, string employeeNetId, string changedByNetId) {
            var job = _directoryRepository.Read(d => d.JobProfiles.Single(jp => jp.Id == jobProfileId));
            var officeId = job.OfficeId;
            var returnValue = await _directoryRepository.DeleteAsync(job);
            _ = await _logHelper.CreateProfileLog(changedByNetId, "Removed Profile", "", employeeId, employeeNetId);
            _ = await _directoryHookHelper.SendHook(employeeId, officeId);
            return returnValue;
        }
    }
}