using System.Security.Cryptography;
using System.Text;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.Helpers {
    public class ApiHelper(DirectoryRepository directoryRepository) {
        private readonly DirectoryRepository _directoryRepository = directoryRepository;

        public async Task<(AreaSettings?, string)> AdvanceApi(int areaId) {
            var sourceItem = await GetAreaSettings(areaId);
            if (sourceItem == null) {
                return (null, "");
            }
            var guid = Guid.NewGuid().ToString().ToLowerInvariant();
            var guidHash = HashWithSHA256(guid);
            sourceItem.ApiSecretPrevious = sourceItem.ApiSecretCurrent;
            sourceItem.ApiSecretCurrent = guidHash;
            sourceItem.ApiSecretLastChanged = DateTime.Now;
            return await _directoryRepository.UpdateAsync(sourceItem) > 0 ? (sourceItem, guid) : (null, "");
        }

        public async Task<bool> CheckApi(int areaId, string key) {
            var sourceItem = await GetAreaSettings(areaId);
            var guidHash = HashWithSHA256(key);
            if (sourceItem == null || string.IsNullOrWhiteSpace(sourceItem.ApiSecretCurrent)) {
                return false;
            }
            return guidHash.Equals(sourceItem.ApiSecretCurrent, StringComparison.OrdinalIgnoreCase) || guidHash.Equals(sourceItem.ApiSecretPrevious, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> CheckApi(string source, string key) {
            var areaId = await _directoryRepository.ReadAsync(d => d.AreaSettings.Where(a => a.InternalCode == source).Select(a => a.AreaId).FirstOrDefault());
            return await CheckApi(areaId, key);
        }

        public async Task<(bool isValid, DateTime lastChanged)> GetApi(int areaId) {
            var sourceItem = await GetAreaSettings(areaId);
            return (sourceItem == null || sourceItem.ApiSecretCurrent == "") ? (false, DateTime.MinValue) : (true, sourceItem.ApiSecretLastChanged ?? DateTime.MinValue);
        }

        public async Task<AreaSettings?> InvalidateApi(int areaId) {
            var sourceItem = await GetAreaSettings(areaId);
            if (sourceItem == null) {
                return null;
            }
            sourceItem.ApiSecretPrevious = "";
            sourceItem.ApiSecretCurrent = "";
            sourceItem.ApiSecretLastChanged = DateTime.Now;
            return (await _directoryRepository.UpdateAsync(sourceItem) > 1 ? sourceItem : null);
        }

        private async Task<AreaSettings?> GetAreaSettings(int areaId) => await _directoryRepository.ReadAsync(d => d.AreaSettings.FirstOrDefault(a => a.AreaId == areaId));
        private static string HashWithSHA256(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value + "-Kindness-Rimless-Cupcake-Untimely")));

    }
}
