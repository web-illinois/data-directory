using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.Data;

namespace uofi_itp_directory_data.Orgchart {
    public class OrgchartHelper(DirectoryRepository directoryRepository) {
        private readonly DirectoryRepository _directoryRepository = directoryRepository;

        public async Task<OrgchartItem?> GetOrgchartForArea(int areaId, bool addEmployees) {
            var area = await _directoryRepository.ReadAsync(d => d.Areas.FirstOrDefault(a => a.Id == areaId));
            if (area == null) {
                return null;
            }
            var offices = await _directoryRepository.ReadAsync(d => d.Offices.Include(o => o.JobProfiles).ThenInclude(jp => jp.EmployeeProfile).Where(o => o.AreaId == areaId).ToList());
            return OrgchartTranslator.TranslateToOrgchartItem(offices, area, addEmployees);
        }

        public async Task<OrgchartItem?> GetOrgchartForOffice(int officeId, bool addEmployees) {
            var office = await _directoryRepository.ReadAsync(d => d.Offices.FirstOrDefault(o => o.Id == officeId));
            if (office == null) {
                return null;
            }
            var offices = await _directoryRepository.ReadAsync(d => d.Offices.Include(o => o.JobProfiles).ThenInclude(jp => jp.EmployeeProfile).Where(o => o.AreaId == office.AreaId).ToList());
            return OrgchartTranslator.TranslateToOrgchartItem(offices, officeId, addEmployees);
        }
    }
}
