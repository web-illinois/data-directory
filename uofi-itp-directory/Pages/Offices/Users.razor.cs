using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory.Controls;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_external.DataWarehouse;

namespace uofi_itp_directory.Pages.Offices {

    public partial class Users {
        private MultiChoice? _multiChoice = default!;
        private List<AreaOfficeThinObject> _officeThinObjects = default!;
        public string Error { get; set; } = "";

        [CascadingParameter]
        public LayoutOffice Layout { get; set; } = default!;

        public string Name { get; set; } = "";
        public string NetId { get; set; } = "";

        public int? OfficeId { get; set; }

        public string OfficeTitle { get; set; } = "Office";
        public List<SecurityEntry> SecurityEntries { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected DataWarehouseManager DataWarehouseManager { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        [Inject]
        protected SecurityEntryHelper SecurityEntryHelper { get; set; } = default!;

        public async Task AssignId() {
            OfficeId = _multiChoice?.SelectedId;
            OfficeTitle = _multiChoice?.SelectedTitle ?? "";
            await AssignTextFields();
        }

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
            if (!string.IsNullOrWhiteSpace(NetId) && OfficeId.HasValue) {
                var (securityEntry, message) = await SecurityEntryHelper.CreateSecurityEntry(NetId, null, OfficeId.Value, await AuthenticationStateProvider.GetUser());
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", message);
                if (securityEntry != null) {
                    SecurityEntries.Add(securityEntry);
                }
                NetId = "";
                StateHasChanged();
            } else {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Net ID needs to be filled out");
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var cachedAreaThinObject = CacheHelper.GetCachedOffice(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder);
            if (cachedAreaThinObject != null) {
                OfficeId = cachedAreaThinObject.Id;
                OfficeTitle = cachedAreaThinObject.Title;
                await AssignTextFields();
            }
            _officeThinObjects = await AccessHelper.GetOffices(await AuthenticationStateProvider.GetAuthenticationStateAsync(), PersonOptionHelper);
            if (_officeThinObjects.IsSingle()) {
                OfficeId = _officeThinObjects.Single().Id;
                OfficeTitle = _officeThinObjects.Single().Title;
                await AssignTextFields();
            }
        }

        private async Task AssignTextFields() {
            if (OfficeId.HasValue) {
                SecurityEntries = [.. (await SecurityEntryHelper.Get(null, OfficeId.Value))];
            }
        }
    }
}