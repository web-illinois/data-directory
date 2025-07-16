using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_external.DataWarehouse;

namespace uofi_itp_directory_data.DataAccess {

    public class AreaHelper(DirectoryRepository directoryRepository, DataWarehouseManager dataWarehouseManager, DirectoryHookHelper directoryHookHelper, LogHelper logHelper) {
        private readonly DataWarehouseManager _dataWarehouseManager = dataWarehouseManager;
        private readonly DirectoryHookHelper _directoryHookHelper = directoryHookHelper;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;
        private readonly LogHelper _logHelper = logHelper;

        public async Task<AreaTag> AddTagToArea(int areaId, string title, bool allowEmployee, ProfileCategoryTypeEnum filter, string changedByNetId, string areaTitle) {
            _ = await _logHelper.CreateAreaLog(changedByNetId, "Added area tag", title, areaId, areaTitle);
            var newTag = new AreaTag { AreaId = areaId, AllowEmployeeToEdit = allowEmployee, Filter = filter, IsActive = true, LastUpdated = DateTime.Now, Title = title };
            _ = await _directoryRepository.CreateAsync(newTag);
            return newTag;
        }

        public async Task<(string, Area?)> GenerateArea(string unitname, string netid, string changedByNetId) {
            var checkExistingArea = await _directoryRepository.ReadAsync(a => a.Areas.FirstOrDefault(a => a.Title == unitname));
            if (checkExistingArea != null)
                return ($"Name '{unitname}' already exists", null);
            var name = await _dataWarehouseManager.GetDataWarehouseItem(netid);
            if (!name.IsValid)
                return ($"Net ID '{netid}' not found", null);
            var area = new Area {
                Title = unitname,
                IsActive = false,
                IsInternalOnly = true,
                AreaSettings = new AreaSettings(),
                Admins = new List<SecurityEntry> { new() { ListedNameLast = name.LastName, ListedNameFirst = name.FirstName, Email = SecurityEntry.TransformName(netid), LastUpdated = DateTime.Now, IsFullAdmin = false, IsActive = true } }
            };
            _ = await _directoryRepository.CreateAsync(area);
            _ = await _logHelper.CreateAreaLog(changedByNetId, "Added area", "", area.Id, area.Title);

            return ($"Unit '{unitname}' created with {name.Name} ({netid}) as an administrator", area);
        }

        public async Task<Area> GetAreaById(int? id, string netId) {
            var area = await _directoryRepository.ReadAsync(d => d.Areas.Single(a => a.Id == id));
            area.IsFullAdmin = await _directoryRepository.ReadAsync(d => d.SecurityEntries.Any(se => se.IsActive && se.IsFullAdmin && se.Email == netId));
            return area;
        }

        public async Task<List<Area>> GetAreas() => [.. (await _directoryRepository.ReadAsync(d => d.Areas.OrderBy(a => a.Title)))];

        public async Task<AreaSettings> GetAreaSettingsByAreaId(int? areaId) => await _directoryRepository.ReadAsync(d => d.AreaSettings.Single(a => a.AreaId == areaId));

        public async Task<AreaSettings> GetAreaSettingsBySource(string source) => await _directoryRepository.ReadAsync(d => d.AreaSettings.SingleOrDefault(a => a.InternalCode == source)) ?? new();

        public async Task<List<AreaTag>> GetAreaTagsByAreaId(int? areaId) => [.. await _directoryRepository.ReadAsync(d => d.AreaTags.Where(a => a.AreaId == areaId).OrderBy(a => a.Title))];

        public async Task<List<Office>> GetOfficesBySource(string source, IEnumerable<string> offices) => await _directoryRepository.ReadAsync(d => d.AreaSettings.Include(a => a.Area).ThenInclude(a => a.Offices).SingleOrDefault(a => a.InternalCode == source)?.Area.Offices.Where(o => o.IsActive && (!offices.Any() || offices.Contains(o.Title))).ToList()) ?? [];

        public async Task<bool> IsCodeUsed(string areaCode) => await _directoryRepository.ReadAsync(d => d.AreaSettings.Any(a => a.InternalCode == areaCode));

        public async Task<int> RemoveArea(Area area, string changedByNetId) {
            foreach (var securityEntry in _directoryRepository.Read(d => d.SecurityEntries.Where(se => se.AreaId == area.Id))) {
                _ = _directoryRepository.Delete(securityEntry);
            }
            area.Admins = [];
            _ = await _logHelper.CreateAreaLog(changedByNetId, "Removed area", area.ToString(), area.Id, area.Title);
            return await _directoryRepository.DeleteAsync(area);
        }

        public async Task<int> RemoveTag(AreaTag areaTag, string changedByNetId, string areaTitle) {
            _ = await _logHelper.CreateAreaLog(changedByNetId, "Removed area tag", areaTag.ToString(), areaTag.AreaId, areaTitle);
            var items = await _directoryRepository.ReadAsync(d => d.JobProfileTags.Include(d => d.JobProfile).Where(a => a.Title == areaTag.Title));
            var employeeIds = items.Select(i => i.JobProfile.EmployeeProfileId).ToArray();

            foreach (var item in items) {
                _ = _directoryRepository.Delete(item);
            }
            _ = await _directoryRepository.DeleteAsync(areaTag);
            return await _directoryHookHelper.PushDirectoryEntry(employeeIds);
        }

        public async Task<int> UpdateArea(Area area, string changedByNetId) {
            _ = await _logHelper.CreateAreaLog(changedByNetId, "Changed area", area.ToString(), area.Id, area.Title);
            return await _directoryRepository.UpdateAsync(area);
        }

        public async Task<int> UpdateAreaSettings(AreaSettings area, string areaName, string changedByNetId) {
            _ = await _logHelper.CreateAreaLog(changedByNetId, "Changed area settings", area.ToString(), area.AreaId, areaName);
            return await _directoryRepository.UpdateAsync(area);
        }
    }
}