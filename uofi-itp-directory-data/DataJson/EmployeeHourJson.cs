using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class EmployeeHourJson {
        public EmployeeHourJson(EmployeeHour eh) {
            if (eh == null)
                return;
            Day = (int)eh.Day;
            EndTime = eh.EndTime;
            Notes = eh.Notes;
            StartTime = eh.StartTime;
        }

        public int Day { get; set; }

        public string EndTime { get; set; } = "";

        public string Notes { get; set; } = "";

        public string StartTime { get; set; } = "";

        public EmployeeHour ToEmployeeHour() {
            return new EmployeeHour {
                Day = (DayOfWeek)Day,
                EndTime = EndTime,
                Notes = Notes,
                StartTime = StartTime
            };
        }
    }
}
