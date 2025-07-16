using Microsoft.AspNetCore.Components;

namespace uofi_itp_directory.Pages.Unit {

    public partial class LayoutUnit {

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private string BaseUrl { get; set; } = "";

        public void Rebuild() {
            BaseUrl = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            StateHasChanged();
        }
    }
}