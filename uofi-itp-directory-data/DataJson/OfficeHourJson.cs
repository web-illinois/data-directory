using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class OfficeHourJson {
        public OfficeHourJson(OfficeHour officeHour) {
            if (officeHour == null)
                return;
            Day = (int)officeHour.Day;
            StartTime = officeHour.StartTime;
            EndTime = officeHour.EndTime;
            Notes = officeHour.Notes;
        }
        public int Day { get; set; }

        public string EndTime { get; set; } = "";

        public string Notes { get; set; } = "";

        public string StartTime { get; set; } = "";

        public OfficeHour ToOfficeHour() {
            return new OfficeHour {
                Day = (DayOfWeek)Day,
                EndTime = EndTime,
                Notes = Notes,
                StartTime = StartTime
            };
        }
    }
}
