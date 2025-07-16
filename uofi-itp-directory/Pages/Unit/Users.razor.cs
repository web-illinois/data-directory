using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory.Controls;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.Pages.Unit {

    public partial class Users {
        private List<AreaOfficeThinObject> _areaThinObjects = default!;
        private MultiChoice? _multiChoice = default!;

        [CascadingParameter]
        public LayoutUnit Layout { get; set; } = default!;

        public string NetId { get; set; } = "";
        public List<SecurityEntry> SecurityEntries { get; set; } = default!;

        [Parameter]
        public int? UnitId { get; set; }

        public string UnitTitle { get; set; } = "Unit";

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        [Inject]
        protected SecurityEntryHelper SecurityEntryHelper { get; set; } = default!;

        public async Task AssignId() {
            UnitId = _multiChoice?.SelectedId;
            UnitTitle = _multiChoice?.SelectedTitle ?? "";
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
            if (!string.IsNullOrWhiteSpace(NetId) && UnitId.HasValue) {
                var (securityEntry, message) = await SecurityEntryHelper.CreateSecurityEntry(NetId, UnitId.Value, null, await AuthenticationStateProvider.GetUser());
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
            var cachedAreaThinObject = CacheHelper.GetCachedArea(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder);
            if (cachedAreaThinObject != null) {
                UnitId = cachedAreaThinObject.Id;
                UnitTitle = cachedAreaThinObject.Title;
            }
            _areaThinObjects = await AccessHelper.GetAreas(await AuthenticationStateProvider.GetAuthenticationStateAsync(), PersonOptionHelper);
            if (_areaThinObjects.IsSingle()) {
                UnitId = _areaThinObjects.Single().Id;
                UnitTitle = _areaThinObjects.Single().Title;
            }
            await AssignTextFields();
        }

        private async Task AssignTextFields() {
            if (UnitId.HasValue) {
                SecurityEntries = [.. (await SecurityEntryHelper.Get(UnitId.Value, null))];
            }
        }
    }
}