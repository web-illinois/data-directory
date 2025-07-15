using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.DirectoryHook;

namespace uofi_itp_directory.Pages.Search {

    public partial class Log {
        public List<DirectoryEntry> Completed { get; set; } = default!;

        [CascadingParameter]
        public LayoutSearch Layout { get; set; } = default!;

        public string NewNetId { get; set; } = "";
        public int NumberUnprocessedItems { get; set; }

        [Inject]
        protected DirectoryHookHelper DirectoryHookHelper { get; set; } = default!;

        [Inject]
        protected EmployeeHelper EmployeeHelper { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        public async Task RefreshInformation() {
            NumberUnprocessedItems = await DirectoryHookHelper.GetUnprocessedCount();
            Completed = await DirectoryHookHelper.GetProcessed();
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            if (!string.IsNullOrWhiteSpace(NewNetId)) {
                var id = await EmployeeHelper.GetEmployeeByNetId(NewNetId);
                if (id != 0) {
                    _ = await DirectoryHookHelper.PushDirectoryEntry([id]);
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Added to queue");
                    await RefreshInformation();
                    NewNetId = "";
                } else {
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Name not found");
                }
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            await RefreshInformation();
        }
    }
}