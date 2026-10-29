using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using uofi_itp_directory_data.Helpers;

namespace uofi_itp_directory_data.DataModels {

    public class AreaTag : BaseDataItem {
        public bool AllowEmployeeToEdit { get; set; }
        public int AreaId { get; set; }
        public ProfileCategoryTypeEnum Filter { get; set; }

        public string FullTitle => $"{Title}{(Filter == ProfileCategoryTypeEnum.None ? "" : " - " + Filter.ToPrettyString() + " only")} {(AllowEmployeeToEdit ? "(employees can edit)" : "(employees cannot edit)")}";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public string Title { get; set; } = "";

        public bool IsShown(ProfileCategoryTypeEnum filterCheck) => Filter == filterCheck || Filter == ProfileCategoryTypeEnum.None;

        public bool IsShown(ProfileCategoryTypeEnum[] filterCheck) => filterCheck.Any(fc => Filter == fc || Filter == ProfileCategoryTypeEnum.None);
    }
}