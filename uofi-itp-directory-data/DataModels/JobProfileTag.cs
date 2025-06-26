using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uofi_itp_directory_data.DataModels {

    public class JobProfileTag : BaseDataItem {
        public bool AllowEmployeeToEdit { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public virtual JobProfile JobProfile { get; set; } = default!;
        public int JobProfileId { get; set; }

        public string Title { get; set; } = "";
    }
}