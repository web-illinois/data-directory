using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory.Controls;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.Pages.Offices {

    public partial class Internal {
        private bool _isDirty = false;
        private MultiChoice? _multiChoice = default!;

        private List<AreaOfficeThinObject> _officeThinObjects = default!;

        [CascadingParameter]
        public LayoutOffice Layout { get; set; } = default!;

        public Office Office { get; set; } = default!;

        [Parameter]
        public int? OfficeId { get; set; }

        public OfficeSettings OfficeSettings { get; set; } = default!;

        public string OfficeTitle { get; set; } = "Office";

        public int PublishingLocation { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected OfficeHelper OfficeHelper { get; set; } = default!;

        [Inject]
        protected PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        [Inject]
        protected SecurityEntryHelper SecurityEntryHelper { get; set; } = default!;

        public async Task AssignId() {
            OfficeId = _multiChoice?.SelectedId;
            OfficeTitle = _multiChoice?.SelectedTitle ?? "";
            await AssignTextFields();
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            Office.IsActive = PublishingLocation > 0 && PublishingLocation != 9;
            Office.CanAddPeople = PublishingLocation != 9;
            Office.IsInternalOnly = PublishingLocation == 1;
            _ = OfficeHelper.UpdateOffice(Office, await AuthenticationStateProvider.GetUser());
            _ = OfficeHelper.UpdateOfficeSettings(OfficeSettings, Office.Title, await AuthenticationStateProvider.GetUser());
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information updated");
            _isDirty = false;
            StateHasChanged();
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

        protected void SetDirty() => _isDirty = true;

        private static int SetPublishingLocation(Office office) {
            if (!office.IsActive && !office.CanAddPeople) {
                return 9;
            }
            if (!office.IsActive) {
                return 0;
            }
            if (office.IsInternalOnly) {
                return 1;
            }
            return 2;
        }

        private async Task AssignTextFields() {
            if (OfficeId.HasValue) {
                Office = await OfficeHelper.GetOfficeById(OfficeId.Value, await AuthenticationStateProvider.GetUser());
                OfficeSettings = await OfficeHelper.GetOfficeSettingsById(Office.Id);
                PublishingLocation = SetPublishingLocation(Office);
            }
        }

        private async Task LocationChangingHandler(LocationChangingContext arg) {
            if (_isDirty) {
                if (!(await JsRuntime.InvokeAsync<bool>("confirm", $"You have unsaved changes. Are you sure?"))) {
                    arg.PreventNavigation();
                }
            }
        }
    }
}