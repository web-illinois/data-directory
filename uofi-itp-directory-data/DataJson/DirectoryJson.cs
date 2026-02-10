namespace uofi_itp_directory_data.DataJson {
    public class DirectoryJson {
        public List<EmployeeJson> Employees { get; set; } = new();
        public List<AreaJson> Areas { get; set; } = new();

        public void RestoreEmployeeIdForJobProfiles(string netId, int employeeId) {
            foreach (var jobProfile in Areas.SelectMany(a => a.Offices).SelectMany(o => o.JobProfiles).Where(jp => jp.NetId == netId)) {
                jobProfile.EmployeeProfileId = employeeId;
            }
        }

        public bool DeleteAllEntries { get; set; } = true;
    }
}
