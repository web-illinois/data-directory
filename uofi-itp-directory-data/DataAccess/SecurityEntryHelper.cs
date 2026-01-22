using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_external.DataWarehouse;

namespace uofi_itp_directory_data.DataAccess {

    public class SecurityEntryHelper(DirectoryRepository directoryRepository, DataWarehouseManager dataWarehouseManager, LogHelper logHelper) {
        private readonly DataWarehouseManager _dataWarehouseManager = dataWarehouseManager;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;
        private readonly LogHelper _logHelper = logHelper;

        public async Task<(SecurityEntry?, string)> CreateSecurityEntry(string netid, int? areaId, int? officeId, string changedByNetId) {
            var editOtherPeople = false;
            if (areaId != null)
                editOtherPeople = await _directoryRepository.ReadAsync(a => a.AreaSettings.First(a => a.AreaId == areaId).AllowAdministratorsAccessToPeople);
            else if (officeId != null)
                editOtherPeople = await _directoryRepository.ReadAsync(a => a.Offices.Include(o => o.Area).ThenInclude(a => a.AreaSettings).First(o => o.Id == officeId).Area.AreaSettings.AllowAdministratorsAccessToPeople);

            var name = await _dataWarehouseManager.GetDataWarehouseItem(netid);
            if (!name.IsValid) {
                return (null, $"Net ID '{netid}' not found");
            }
            var securityEntry = new SecurityEntry(netid, name.FirstName, name.LastName, areaId, officeId, editOtherPeople);
            _ = await _directoryRepository.CreateAsync(securityEntry);
            _ = await _logHelper.CreateSecurityLog(changedByNetId, "Created security item", securityEntry?.ToString() ?? "", securityEntry?.Id ?? 0, securityEntry?.Email ?? "");
            return (securityEntry, $"User '{name.Name}' created");
        }

        public async Task<bool> UpdateSecurityEntry(string netid, string firstNameOverride, string lastNameOverride) {
            var securityEntries = await _directoryRepository.ReadAsync(d => d.SecurityEntries.Where(se => se.Email == netid));
            if (securityEntries.Count() == 0) {
                return false;
            }
            var name = await _dataWarehouseManager.GetDataWarehouseItem(netid);
            if (!name.IsValid) {
                return false;
            }
            foreach (var securityEntry in securityEntries) {
                securityEntry.ListedNameFirst = !string.IsNullOrWhiteSpace(firstNameOverride) ? firstNameOverride : name.FirstName;
                securityEntry.ListedNameLast = !string.IsNullOrWhiteSpace(lastNameOverride) ? lastNameOverride : name.LastName;
                _ = await _directoryRepository.UpdateAsync(securityEntry);
            }
            return true;
        }

        public async Task<int> Delete(SecurityEntry? securityEntry, string changedByNetId) {
            _ = await _logHelper.CreateSecurityLog(changedByNetId, "Deleted security item", securityEntry?.ToString() ?? "", securityEntry?.Id ?? 0, securityEntry?.Email ?? "");
            return await _directoryRepository.DeleteAsync(securityEntry);
        }

        public async Task<List<SecurityEntry>> Get(int? areaId, int? officeId) {
            if (officeId != null)
                return [.. (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(se => se.OfficeId == officeId).OrderBy(c => c.ListedNameLast)))];
            else if (areaId != null)
                return [.. (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(se => se.AreaId == areaId).OrderBy(c => c.ListedNameLast)))];
            return [.. (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(se => se.IsFullAdmin).OrderBy(c => c.ListedNameLast)))];
        }

        public async Task<int> SetAccessToOtherPeople(int areaId, bool allowAccess) {
            var returnValue = 0;
            var officeIds = (await _directoryRepository.ReadAsync(c => c.Offices.Where(o => o.AreaId == areaId).Select(o => o.Id))).ToList();
            foreach (var securityAccess in await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(se => se.AreaId == areaId || officeIds.Contains(se.OfficeId ?? -1)))) {
                securityAccess.CanEditAllPeopleInUnit = allowAccess;
                returnValue += await _directoryRepository.UpdateAsync(securityAccess);
            }
            return returnValue;
        }

        public async Task<int> Update(SecurityEntry securityEntry, string changedByNetId) {
            _ = await _logHelper.CreateSecurityLog(changedByNetId, "Updated security item", securityEntry.ToString(), securityEntry.Id, securityEntry.Email);
            return await _directoryRepository.UpdateAsync(securityEntry);
        }
    }
}