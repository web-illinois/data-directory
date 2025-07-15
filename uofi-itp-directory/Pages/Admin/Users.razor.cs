using Azure.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_external.DataWarehouse;

namespace uofi_itp_directory.Pages.Admin {

    public partial class Users {

        [CascadingParameter]
        public LayoutAdmin Layout { get; set; } = default!;

        public string Name { get; set; } = "";
        public string NetId { get; set; } = "";
        public List<SecurityEntry> SecurityEntries { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected DataWarehouseManager DataWarehouseManager { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        [Inject]
        protected SecurityEntryHelper SecurityEntryHelper { get; set; } = default!;

        public Task ChangeEntry() {
            StateHasChanged();
            return Task.CompletedTask;
        }

        public Task RemoveEntry(int id) {
            SecurityEntries.RemoveAll(se => se.Id == id);
            return Task.CompletedTask;
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            if (!string.IsNullOrWhiteSpace(NetId)) {
                var (securityEntry, message) = await SecurityEntryHelper.CreateSecurityEntry(NetId, null, null, await AuthenticationStateProvider.GetUser());
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", message);
                if (securityEntry != null) {
                    SecurityEntries.Add(securityEntry);
                }
                NetId = "";
                StateHasChanged();
            } else {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "NetID is required");
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var authstate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var name = authstate.User?.Identity?.Name;
            if (!await PersonOptionHelper.IsFullAdmin(name)) {
                throw new AuthenticationFailedException("Full Admin access required");
            }
            SecurityEntries = [.. (await SecurityEntryHelper.Get(null, null))];
        }
    }
}