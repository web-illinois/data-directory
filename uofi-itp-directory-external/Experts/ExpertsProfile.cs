namespace uofi_itp_directory_external.Experts {

    public class ExpertsProfile {
        public List<ExpertsItem> Awards { get; set; } = default!;
        public string Biography { get; set; } = "";
        public List<ExpertsItem> Clippings { get; set; } = [];
        public List<ExpertsItem> EducationHistory { get; set; } = default!;
        public string ExpertsId { get; set; } = "";
        public string ExpertsUrl { get; set; } = "";
        public List<ExpertsItem> Grants { get; set; } = default!;
        public List<string> Keywords { get; set; } = [];
        public string LinkedIn { get; internal set; } = "";
        public List<ExpertsItem> Links { get; set; } = [];
        public string NetId { get; internal set; } = "";
        public List<ExpertsItem> Organizations { get; set; } = [];
        public List<ExpertsItem> Presentations { get; set; } = [];
        public List<ExpertsItem> Publications { get; set; } = [];
        public string Quote { get; set; } = "";
        public string ResearchStatement { get; set; } = "";
        public List<ExpertsItem> Services { get; set; } = default!;
        public string TeachingStatement { get; set; } = "";
        public string Twitter { get; set; } = "";
        public bool UseExperts => !string.IsNullOrWhiteSpace(ExpertsId);
    }
}