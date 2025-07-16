using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace uofi_itp_directory.Shared {

    public partial class SidebarLayout {

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                _ = await JsRuntime.InvokeAsync<bool>("blazorMenu");
            }
        }
    }
}