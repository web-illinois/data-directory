using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class JobProfileJson {
        public JobProfileJson(JobProfile jobProfile) {
            if (jobProfile == null)
                return;
            Category = (int)jobProfile.Category;
            Description = jobProfile.Description;
            InternalOrder = jobProfile.InternalOrder;
            NetId = jobProfile.EmployeeProfile?.NetId ?? "";
            Tags = [.. jobProfile.Tags?.Select(t => new JobProfileTagJson(t)) ?? []];
            Title = jobProfile.Title;
        }
        public int Category { get; set; }
        public string Description { get; set; } = "";
        public int InternalOrder { get; set; }
        public string NetId { get; set; } = "";
        public int EmployeeProfileId { get; set; }

        public List<JobProfileTagJson> Tags { get; set; } = [];

        public string Title { get; set; } = "";

        public JobProfile ToJobProfile() => new() {
            Category = (ProfileCategoryTypeEnum)Category,
            Description = Description,
            InternalOrder = InternalOrder,
            Title = Title,
            Tags = [.. Tags?.Select(t => t.ToJobProfileTag()) ?? []],
            EmployeeProfileId = EmployeeProfileId
        };
    }
}
