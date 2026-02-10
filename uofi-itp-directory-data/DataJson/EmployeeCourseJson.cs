using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class EmployeeCourseJson {
        public EmployeeCourseJson(EmployeeCourse ec) {
            if (ec == null) {
                return;
            }
            CourseNumber = ec.CourseNumber;
            Description = ec.Description;
            InternalOrder = ec.InternalOrder;
            Rubric = ec.Rubric;
            Title = ec.Title;
            Url = ec.Url;
        }

        public string CourseNumber { get; set; } = "";
        public string Description { get; set; } = "";
        public int InternalOrder { get; set; }
        public string Rubric { get; set; } = "";
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";

        public EmployeeCourse ToEmployeeCourse() {
            return new EmployeeCourse {
                CourseNumber = CourseNumber,
                Description = Description,
                InternalOrder = InternalOrder,
                Rubric = Rubric,
                Title = Title,
                Url = Url
            };
        }
    }
}
