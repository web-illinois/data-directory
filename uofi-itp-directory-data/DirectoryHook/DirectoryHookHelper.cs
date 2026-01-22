using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DirectoryHook {

    public class DirectoryHookHelper {
        private readonly DirectoryRepository _directoryRepository;
        private readonly string _netIdPlaceholder;
        private readonly string _sourcePlaceholder;
        private readonly string _url;

        public DirectoryHookHelper(DirectoryRepository? directoryRepository, string? url) {
            ArgumentNullException.ThrowIfNull(directoryRepository);
            _directoryRepository = directoryRepository;
            _netIdPlaceholder = "{netid}";
            _sourcePlaceholder = "{source}";
            _url = url ?? "";
        }

        public async Task<List<DirectoryEntry>> GetProcessed() => [.. await _directoryRepository.ReadAsync(d => d.DirectoryEntries
                .Where(de => de.DateRun != null).OrderByDescending(de => de.DateRun))];

        public async Task<int> GetUnprocessedCount() => await _directoryRepository.ReadAsync(d => d.DirectoryEntries
            .Where(de => de.DateRun == null).Count());

        public async Task<int> LoadAreas() {
            var returnValue = 0;
            _ = _directoryRepository.DeleteAllDirectoryEntries();
            var areasToAdd = (await _directoryRepository.ReadAsync(d => d.AreaSettings.Where(a => a.AutoloadProfiles && a.UrlPeopleRefreshType != PeopleRefreshTypeEnum.None).Select(a => a.AreaId))).ToList();
            foreach (var employeeId in await _directoryRepository.ReadAsync(d => d.JobProfiles.Include(jp => jp.Office)
                .Where(jp => areasToAdd.Contains(jp.Office.AreaId))
                .Select(jp => jp.EmployeeProfileId).Distinct().ToList())) {
                returnValue += await _directoryRepository.CreateAsync(new DirectoryEntry(employeeId));
            }
            return returnValue;
        }

        public async Task<DirectoryEntry> PopDirectoryEntry(DirectoryEntry entry) {
            if (entry == null) {
                return new DirectoryEntry(0);
            }
            var (successful, netId, results) = await SendHook(entry.EmployeeId, false);
            entry.DateRun = DateTime.Now;
            entry.LastUpdated = DateTime.Now;
            entry.IsSuccessful = successful;
            entry.NetId = netId;
            entry.Message = results;
            _ = await _directoryRepository.UpdateAsync(entry);
            return entry;
        }

        public async Task<int> PushDirectoryEntry(int[] employeeIds) {
            var returnValue = 0;
            foreach (var employeeId in employeeIds.Distinct()) {
                returnValue += await _directoryRepository.CreateAsync(new DirectoryEntry(employeeId, true));
            }
            return returnValue;
        }

        public async Task<(bool isSuccessful, string netid, string results)> SendHook(int employeeId, bool ignoreResults) {
            var employee = await _directoryRepository.ReadAsync(d => d.Employees.Include(e => e.JobProfiles).ThenInclude(jp => jp.Office).SingleOrDefault(e => e.Id == employeeId));
            return await SendHookPrivate(employee, ignoreResults);
        }

        public async Task<(bool isSuccessful, string netid, string results)> SendHookToRemoveEmployee(int employeeId, string netId, int officeId) {
            // need to rebuild fake employee with office information to send a "this employee should be removed" message
            var employee = await _directoryRepository.ReadAsync(d => d.Employees.SingleOrDefault(e => e.Id == employeeId));
            var office = await _directoryRepository.ReadAsync(d => d.Offices.FirstOrDefault(o => o.Id == officeId));
            employee ??= new Employee { NetId = netId };
            employee.JobProfiles = office == null ? new List<JobProfile>() : new List<JobProfile> { new() { Office = office } };
            return await SendHookPrivate(employee, false);
        }

        private async Task<(bool isSuccessful, string netid, string results)> SendHookPrivate(Employee? employee, bool ignoreResults) {
            if (employee == null) {
                return (false, "", "employee not found");
            }
            var netId = employee.NetId.Replace("@illinois.edu", "");
            var areAllSuccessful = true;
            var results = "";
            var codesUsed = new List<string>();
            foreach (var profile in employee.JobProfiles.Where(jp => jp != null && jp.Office != null)) {
                var areaSettings = await _directoryRepository.ReadAsync(d => d.AreaSettings.SingleOrDefault(a => a.AreaId == profile.Office.AreaId));
                if (areaSettings?.UrlPeopleRefreshType == PeopleRefreshTypeEnum.Custom) {
                    var url = areaSettings?.UrlPeopleRefreshFullUrl.Replace(_netIdPlaceholder, netId).Replace(_sourcePlaceholder, areaSettings?.InternalCode) ?? "";
                    using var client = new HttpClient();
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    if (ignoreResults) {
                        _ = await client.SendAsync(requestMessage);
                        results += $"{profile.Office.Title}: sent. ";
                    } else {
                        using var res = await client.SendAsync(requestMessage);
                        areAllSuccessful = areAllSuccessful || res.IsSuccessStatusCode;
                        results += $"{profile.Office.Title}: {await res.Content.ReadAsStringAsync().ConfigureAwait(false) ?? ""}. ";
                    }
                } else if (areaSettings?.UrlPeopleRefreshType == PeopleRefreshTypeEnum.Default && codesUsed.Contains(areaSettings?.InternalCode ?? "")) {
                    results += $"{profile.Office.Title}: {areaSettings?.InternalCode} already sent. ";
                } else if (areaSettings?.UrlPeopleRefreshType == PeopleRefreshTypeEnum.Default) {
                    var url = _url.Replace(_netIdPlaceholder, netId).Replace(_sourcePlaceholder, areaSettings?.InternalCode) ?? "";
                    using var client = new HttpClient();
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    if (ignoreResults) {
                        _ = await client.SendAsync(requestMessage);
                        results += $"{profile.Office.Title}: sent. ";
                    } else {
                        using var res = await client.SendAsync(requestMessage);
                        areAllSuccessful = areAllSuccessful || res.IsSuccessStatusCode;
                        results += $"{profile.Office.Title}: {await res.Content.ReadAsStringAsync().ConfigureAwait(false) ?? ""}. ";
                    }
                }
            }
            await UpdateLastUpdated(employee);
            return (areAllSuccessful, netId, results);
        }

        private async Task<bool> UpdateLastUpdated(Employee employee) {
            employee.LastRefreshed = DateTime.Now;
            return await _directoryRepository.UpdateAsync(employee) > 0;
        }
    }
}