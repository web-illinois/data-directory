using Microsoft.AspNetCore.Components;
using uofi_itp_directory_data.Helpers;

namespace uofi_itp_directory.Pages.Search {
    public partial class ChangeLog {
        public List<uofi_itp_directory_data.DataModels.Log> LogItems { get; set; } = default!;

        public string SearchText { get; set; } = string.Empty;

        [CascadingParameter]
        public LayoutSearch Layout { get; set; } = default!;

        [Inject]
        public LogReaderHelper LogReaderHelper { get; set; } = default!;

        public async Task RefreshInformation() {
            LogItems = await LogReaderHelper.GetLogs(SearchText);
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            await RefreshInformation();
        }
    }
}
