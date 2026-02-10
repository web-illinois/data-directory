using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class JobProfileTagJson {
        public JobProfileTagJson(JobProfileTag jobProfile) {
            if (jobProfile == null) {
                return;
            }
            AllowEmployeeToEdit = jobProfile.AllowEmployeeToEdit;
            Title = jobProfile.Title;
        }
        public bool AllowEmployeeToEdit { get; set; }
        public string Title { get; set; } = "";

        public JobProfileTag ToJobProfileTag() {
            return new JobProfileTag {
                AllowEmployeeToEdit = AllowEmployeeToEdit,
                Title = Title
            };
        }
    }
}
