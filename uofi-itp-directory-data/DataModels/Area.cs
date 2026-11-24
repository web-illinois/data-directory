using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uofi_itp_directory_data.DataModels {

    public class Area : BaseDataItem {
        public virtual ICollection<SecurityEntry> Admins { get; set; } = default!;

        public virtual ICollection<AreaJobType> AreaJobTypes { get; set; } = default!;
        public virtual AreaSettings AreaSettings { get; set; } = default!;
        public virtual ICollection<AreaTag> AreaTags { get; set; } = default!;
        public AreaTypeEnum AreaType { get; set; }

        public string Audience { get; set; } = "";

        public string ExternalUrl { get; set; } = "";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public int InternalOrder { get; set; } = 2;

        public string InternalUrl { get; set; } = "";

        [NotMapped]
        public bool IsFullAdmin { get; set; } // used by the code to determine if the user can edit order information

        public bool IsInternalOnly { get; set; }
        public int? ManagerRootOfficeId { get; set; } = null;
        public string Notes { get; set; } = "";
        public virtual ICollection<Office> Offices { get; set; } = default!;
        public string Title { get; set; } = "";
    }
}