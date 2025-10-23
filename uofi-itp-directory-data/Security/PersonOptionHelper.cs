using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.Data;

namespace uofi_itp_directory_data.Security {

    public class PersonOptionHelper(DirectoryRepository directoryRepository) {
        private readonly DirectoryRepository _directoryRepository = directoryRepository;

        public async Task<IEnumerable<AreaOfficeThinObject>> Areas(string? name) {
            if (!CheckParameters(name)) {
                return new List<AreaOfficeThinObject>();
            }
            var items = await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(p => p.Email == name && p.IsActive).ToList());
            if (items.Any(i => i.IsFullAdmin)) {
                return (await _directoryRepository.ReadAsync(a => a.Areas.Select(a => new AreaOfficeThinObject(a)))).ToList();
            }
            return (await _directoryRepository.ReadAsync(a => a.Areas.Where(a => items.Select(item => item.AreaId).Contains(a.Id)).Select(a => new AreaOfficeThinObject(a)))).ToList();
        }

        public async Task<FullSecurityItem> GetSecurityItem(string? name) {
            if (!CheckParameters(name)) {
                return new FullSecurityItem { IsFullAdmin = false, IsOfficeAdmin = false, IsUnitAdmin = false, HasProfile = false };
            }
            var securityEntry = (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(p => p.Email == name && p.IsActive))).ToList();
            var returnValue = securityEntry == null || !securityEntry.Any()
                ? new FullSecurityItem { IsFullAdmin = false, IsOfficeAdmin = false, IsUnitAdmin = false }
                : new FullSecurityItem { IsFullAdmin = securityEntry.Any(s => s.IsFullAdmin), IsOfficeAdmin = true, IsUnitAdmin = securityEntry.Any(s => s.IsFullAdmin) || securityEntry.Any(s => s.AreaId.HasValue) };

            returnValue.HasProfile = await _directoryRepository.ReadAsync(c => c.JobProfiles.Include(c => c.EmployeeProfile).Any(c => c.EmployeeProfile.NetId == name));
            return returnValue;
        }

        public async Task<bool> IsAreaAdmin(string? name, int areaId) => CheckParameters(name) &&
                            (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Any(p => p.Email == name && p.IsActive && (p.IsFullAdmin || p.AreaId == areaId))));

        public async Task<bool> IsFullAdmin(string? name) => CheckParameters(name) &&
                    (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Any(p => p.Email == name && p.IsActive && p.IsFullAdmin)));

        public async Task<bool> IsOfficeAdmin(string? name, int officeId) {
            if (!CheckParameters(name)) {
                return false;
            }
            var areaId = _directoryRepository.Read(c => c.Offices.Single(o => o.Id == officeId).AreaId);
            return await _directoryRepository.ReadAsync(c => c.SecurityEntries.Any(p => p.Email == name && p.IsActive && (p.IsFullAdmin || p.AreaId == areaId || p.OfficeId == officeId)));
        }

        // Note that this looks only at office, not areas or full admin access -- running this against areas should be done first
        public async Task<IEnumerable<AreaOfficeThinObject>> Offices(string? name) {
            if (!CheckParameters(name)) {
                return new List<AreaOfficeThinObject>();
            }
            var items = (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(p => p.Email == name && p.IsActive))).ToList();
            if (items.Any(i => i.IsFullAdmin)) {
                return (await _directoryRepository.ReadAsync(o => o.Offices.Include(o => o.Area).Select(o => new AreaOfficeThinObject(o)))).ToList();
            }
            return (await _directoryRepository.ReadAsync(o => o.Offices.Include(o => o.Area).Where(o => items.Select(item => item.AreaId).Contains(o.AreaId) || items.Select(item => item.OfficeId).Contains(o.Id)).Distinct().Select(o => new AreaOfficeThinObject(o)))).ToList();
        }

        private static bool CheckParameters(string? name) => !string.IsNullOrWhiteSpace(name);
    }
}