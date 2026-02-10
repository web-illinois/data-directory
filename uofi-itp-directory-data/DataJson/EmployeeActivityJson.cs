using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class EmployeeActivityJson {
        public EmployeeActivityJson(EmployeeActivity ea) {
            if (ea == null) {
                return;
            }
            InternalOrder = ea.InternalOrder;
            Title = ea.Title;
            Type = (int)ea.Type;
            Url = ea.Url;
            YearEnded = ea.YearEnded;
            YearStarted = ea.YearStarted;
        }

        public int InternalOrder { get; set; }
        public string Title { get; set; } = "";
        public int Type { get; set; }
        public string Url { get; set; } = "";

        public string YearEnded { get; set; } = "";
        public string YearStarted { get; set; } = "";

        public EmployeeActivity ToEmployeeActivity() {
            return new EmployeeActivity {
                InternalOrder = InternalOrder,
                Title = Title,
                Type = (ActivityTypeEnum)Type,
                Url = Url,
                YearEnded = YearEnded,
                YearStarted = YearStarted
            };
        }
    }
}
