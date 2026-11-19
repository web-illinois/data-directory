using Microsoft.AspNetCore.Components;
using uofi_itp_directory_data.WebTools;

namespace uofi_itp_directory.Pages.Search {
    public partial class WebtoolsLog {
        public List<uofi_itp_directory_data.DataModels.ToolkitLog> LogItems { get; set; } = default!;

        public string SearchText { get; set; } = string.Empty;

        [CascadingParameter]
        public LayoutSearch Layout { get; set; } = default!;

        [Inject]
        public WebtoolsLookup WebtoolsLookup { get; set; } = default!;

        public async Task RefreshInformation() {
            LogItems = await WebtoolsLookup.GetLogs(SearchText);
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            await RefreshInformation();
        }
    }
}
