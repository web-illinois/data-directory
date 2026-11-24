using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.Orgchart {
    public static class OrgchartTranslator {

        public static OrgchartItem? TranslateToOrgchartItem(List<Office> Offices, Area area, bool addEmployees) {
            return TranslateToOrgchartItem(Offices, area.ManagerRootOfficeId ?? 0, addEmployees);
        }

        public static OrgchartItem? TranslateToOrgchartItem(List<Office> Offices, int Id, bool addEmployees) {
            var primaryOffice = Offices.Find(o => o.Id == Id);
            if (primaryOffice == null) {
                return null;
            }
            var primaryJob = primaryOffice?.JobProfiles.FirstOrDefault(jp => jp.Id == primaryOffice.ManagerJobProfileId1);
            var item = new OrgchartItem {
                Title = GetTitleForOffice(primaryJob, primaryOffice),
                Subtitle = GetSubtitleForOffice(primaryJob, primaryOffice),
                Link = primaryJob == null ? primaryOffice?.ExternalUrl ?? "" : primaryJob?.EmployeeProfile.ProfileUrl,
                Large = true,
                Weight = 0,
                Children = new List<OrgchartItem>()
            };
            var secondaryJob = primaryOffice?.JobProfiles.FirstOrDefault(jp => jp.Id == primaryOffice.ManagerJobProfileId2);
            if (secondaryJob != null) {
                var secondaryItem = new OrgchartItem {
                    Title = GetTitleForOffice(secondaryJob, primaryOffice),
                    Subtitle = GetSubtitleForOffice(secondaryJob, primaryOffice),
                    Link = secondaryJob?.EmployeeProfile?.ProfileUrl,
                    Large = false,
                    Weight = -1,
                    Parent = item
                };
                item.Children.Add(secondaryItem);
            }
            foreach (var office in Offices) {
                if (office.ManagerOfficeId == Id) {
                    var childItem = TranslateToOrgchartItem(Offices, office.Id, addEmployees);
                    if (childItem == null) {
                        continue;
                    }
                    childItem.Parent = item;
                    item.Children.Add(childItem);
                }
            }
            if (addEmployees) {
                foreach (var job in primaryOffice?.JobProfiles ?? new List<JobProfile>()) {
                    if (job.Id != primaryOffice?.ManagerJobProfileId1 && job.Id != primaryOffice?.ManagerJobProfileId2) {
                        var childItem = new OrgchartItem {
                            Title = GetTitleForOffice(job, primaryOffice),
                            Subtitle = GetSubtitleForOffice(job, primaryOffice),
                            Link = job?.EmployeeProfile?.ProfileUrl,
                            Large = false,
                            Weight = 0,
                            Parent = item
                        };
                        item.Children.Add(childItem);
                    }
                }
            }
            return item;
        }
        private static string GetTitleForOffice(JobProfile? primaryJob, Office? office) {
            if (primaryJob == null) {
                return office?.Title ?? "";
            }
            if (!string.IsNullOrWhiteSpace(office?.ManagerOverride1)) {
                return office.ManagerOverride1;
            }
            return primaryJob.EmployeeProfile.Name ?? "";
        }

        private static string GetSubtitleForOffice(JobProfile? primaryJob, Office? office) {
            if (primaryJob == null) {
                return office?.Title ?? "";
            }
            if (!string.IsNullOrWhiteSpace(office?.ManagerOverride1)) {
                return office.ManagerOverride2;
            }
            return office?.ManagerIncludeOffice ?? false ? $"{primaryJob?.Title}, {office?.Title}" : primaryJob?.Title ?? "";
        }
    }
}
