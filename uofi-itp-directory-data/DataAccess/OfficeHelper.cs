using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_external.DataWarehouse;

namespace uofi_itp_directory_data.DataAccess {

    public class OfficeHelper(DirectoryRepository directoryRepository, DataWarehouseManager dataWarehouseManager, LogHelper logHelper) {
        private readonly DataWarehouseManager _dataWarehouseManager = dataWarehouseManager;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;
        private readonly LogHelper _logHelper = logHelper;

        public async Task<(string, Office?)> GenerateOffice(string officename, int areaId, string netid, string changedByNetId) {
            var checkExistingArea = await _directoryRepository.ReadAsync(a => a.Offices.FirstOrDefault(a => a.Title == officename && a.AreaId == areaId));
            if (checkExistingArea != null)
                return ($"Name '{officename}' already exists", null);
            var name = await _dataWarehouseManager.GetDataWarehouseItem(netid);
            if (!name.IsValid) {
                return ($"Net ID '{netid}' not found", null);
            }
            var office = new Office {
                Title = officename,
                AreaId = areaId,
                IsActive = true,
                IsInternalOnly = true,
                OfficeSettings = new OfficeSettings { IsActive = true },
                OfficeHours = new List<OfficeHour> { new() { Day = DayOfWeek.Sunday }, new() { Day = DayOfWeek.Monday }, new() { Day = DayOfWeek.Tuesday }, new() { Day = DayOfWeek.Wednesday }, new() { Day = DayOfWeek.Thursday }, new() { Day = DayOfWeek.Friday }, new() { Day = DayOfWeek.Saturday } },
                Admins = new List<SecurityEntry> { new() { ListedNameLast = name.LastName, ListedNameFirst = name.FirstName, Email = SecurityEntry.TransformName(netid), LastUpdated = DateTime.Now, IsFullAdmin = false, IsActive = true } }
            };
            _ = await _directoryRepository.CreateAsync(office);
            _ = await _logHelper.CreateOfficeLog(changedByNetId, "Added office", "", office.Id, office.Title);
            return ($"Office '{officename}' created with {name.Name} ({netid}) as an administrator", office);
        }

        public async Task<Office> GetOfficeById(int id, string netId) {
            var office = await _directoryRepository.ReadAsync(d => d.Offices.Single(a => a.Id == id));
            office.IsAreaAdmin = await _directoryRepository.ReadAsync(d => d.SecurityEntries.Any(se => se.IsActive && se.Email == netId && (se.IsFullAdmin || se.AreaId == office.AreaId)));
            return office;
        }

        public async Task<List<OfficeHour>> GetOfficeHoursById(int officeId) => [.. await _directoryRepository.ReadAsync(d => d.OfficeHours.Where(oh => oh.OfficeId == officeId).OrderBy(oh => oh.Day))];

        public async Task<List<Office>> GetOffices(int areaId) => [.. (await _directoryRepository.ReadAsync(d => d.Offices.Where(o => o.AreaId == areaId).OrderBy(a => a.Title)))];

        public async Task<bool> GetOfficeSettingJobDescriptionById(int officeId) => await _directoryRepository.ReadAsync(d => d.OfficeSettings.Single(o => o.OfficeId == officeId).UseJobSpecificDescription);

        public async Task<OfficeSettings> GetOfficeSettingsById(int officeId) => await _directoryRepository.ReadAsync(d => d.OfficeSettings.Single(o => o.OfficeId == officeId));

        public async Task<bool> IsCodeUsed(string officeCode) => await _directoryRepository.ReadAsync(d => d.OfficeSettings.Any(a => a.InternalCode == officeCode));

        public async Task<int> RemoveOffice(Office office, string changedByNetId) {
            foreach (var securityEntry in _directoryRepository.Read(d => d.SecurityEntries.Where(se => se.OfficeId == office.Id))) {
                _directoryRepository.Delete(securityEntry);
            }
            office.Admins = Array.Empty<SecurityEntry>();
            _ = await _logHelper.CreateOfficeLog(changedByNetId, "Removed office", office.ToString(), office.Id, office.Title);
            return await _directoryRepository.DeleteAsync(office);
        }

        public async Task<int> UpdateOffice(Office office, string changedByNetId) {
            _ = await _logHelper.CreateOfficeLog(changedByNetId, "Changed office", office.ToString(), office.Id, office.Title);
            return await _directoryRepository.UpdateAsync(office);
        }

        public async Task<int> UpdateOfficeHour(OfficeHour office, string officeName, string changedByNetId) {
            _ = await _logHelper.CreateOfficeLog(changedByNetId, "Changed office hour", office.ToString(), office.OfficeId, officeName);
            return await _directoryRepository.UpdateAsync(office);
        }

        public async Task<int> UpdateOfficeSettings(OfficeSettings office, string officeName, string changedByNetId) {
            _ = await _logHelper.CreateOfficeLog(changedByNetId, "Changed office settings", office.ToString(), office.OfficeId, officeName);
            return await _directoryRepository.UpdateAsync(office);
        }
    }
}