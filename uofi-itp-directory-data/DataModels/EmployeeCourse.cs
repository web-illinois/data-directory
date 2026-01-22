using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uofi_itp_directory_data.DataModels {
    public class EmployeeCourse : BaseDataItem {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public string CourseNumber { get; set; } = "";
        public string Description { get; set; } = "";
        public virtual Employee Employee { get; set; } = default!;

        public int EmployeeId { get; set; }
        [NotMapped]
        public bool InEditState { get; set; } = false;
        public int InternalOrder { get; set; }
        public string Rubric { get; set; } = "";
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
    }
}
