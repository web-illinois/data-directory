using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class AreaTagJson {
        public AreaTagJson(AreaTag areaTag) {
            if (areaTag == null) {
                return;
            }
            AllowEmployeeToEdit = areaTag.AllowEmployeeToEdit;
            Filter = (int)areaTag.Filter;
            Title = areaTag.Title;
        }
        public bool AllowEmployeeToEdit { get; set; }
        public int Filter { get; set; }
        public string Title { get; set; } = "";

        public AreaTag ToAreaTag() => new() { AllowEmployeeToEdit = AllowEmployeeToEdit, Filter = (ProfileCategoryTypeEnum)Filter, Title = Title };
    }
}
