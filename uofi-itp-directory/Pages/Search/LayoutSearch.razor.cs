using Microsoft.AspNetCore.Components;

namespace uofi_itp_directory.Pages.Search {
    public partial class LayoutSearch {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private string BaseUrl { get; set; } = "";

        public void Rebuild() {
            BaseUrl = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            StateHasChanged();
        }
    }
}
