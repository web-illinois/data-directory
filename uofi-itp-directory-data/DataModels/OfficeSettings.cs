using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uofi_itp_directory_data.DataModels {

    public class OfficeSettings : BaseDataItem {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public string InternalCode { get; set; } = "";
        public string InternalNotes { get; set; } = "";
        public bool WebToolsAutomaticImport { get; set; } = false;
        public bool WebToolsAutomaticEmployeeImport { get; set; } = false;

        public virtual Office Office { get; set; } = default!;

        public int OfficeId { get; set; }

        public bool UseJobSpecificDescription { get; set; }
    }
}