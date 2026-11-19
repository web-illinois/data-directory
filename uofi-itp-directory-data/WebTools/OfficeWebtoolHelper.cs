using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.WebTools {
    public class OfficeWebtoolHelper(DirectoryRepository? directoryRepository, WebtoolsImporter webtoolsImporter) {
        private readonly DirectoryRepository _directoryRepository = directoryRepository ?? throw new ArgumentNullException("directoryRepository");
        private readonly WebtoolsImporter _webtoolsImporter = webtoolsImporter ?? throw new ArgumentNullException("webtoolsImporter");

        public async Task<string> ImportOffice() {
            var returnValue = "";
            var offices = await _directoryRepository.ReadAsync(d => d.Offices.Include(o => o.OfficeSettings).Where(o => o.OfficeSettings.WebToolsAutomaticImport));
            foreach (var office in offices) {
                var value = "";
                if (!string.IsNullOrWhiteSpace(office.DepartmentId)) {
                    value = await _webtoolsImporter.ImportDepartment(office?.DepartmentId ?? "", office?.Id ?? 0, "automated", office?.OfficeSettings?.WebToolsAutomaticEmployeeImport ?? false);
                } else if (!string.IsNullOrWhiteSpace(office.ServiceId)) {
                    value = await _webtoolsImporter.ImportService(office?.ServiceId ?? "", office?.Id ?? 0, "automated", office?.OfficeSettings?.WebToolsAutomaticEmployeeImport ?? false);
                }
                _ = await _directoryRepository.CreateAsync(new ToolkitLog {
                    IsActive = true,
                    LastUpdated = DateTime.Now,
                    Log = value,
                    IsLast = false,
                    ToolkitType = ToolkitTypeEnum.Office
                });
                returnValue += $"{value}; ";
            }
            return returnValue;
        }

    }
}
