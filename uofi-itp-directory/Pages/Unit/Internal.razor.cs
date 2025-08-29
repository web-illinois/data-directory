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

namespace uofi_itp_directory.Pages.Unit {

    public partial class Internal {
        private List<AreaOfficeThinObject> _areaThinObjects = default!;
        private bool _isDirty = false;
        private MultiChoice? _multiChoice = default!;

        public Area Area { get; set; } = default!;

        public AreaSettings AreaSettings { get; set; } = default!;

        [CascadingParameter]
        public LayoutUnit Layout { get; set; } = default!;

        public int ProfileInformation { get; set; }

        public int PublishingLocation { get; set; }

        public int? UnitId { get; set; }

        public string UnitTitle { get; set; } = "Unit";

        [Inject]
        protected AreaHelper AreaHelper { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected EmployeeHelper EmployeeHelper { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        [Inject]
        protected SecurityEntryHelper SecurityEntryHelper { get; set; } = default!;

        private bool _originalAllowAccess { get; set; }

        private string _originalUrlProfile { get; set; } = "";

        public async Task AssignId() {
            UnitId = _multiChoice?.SelectedId;
            UnitTitle = _multiChoice?.SelectedTitle ?? "";
            await AssignTextFields();
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            Area.IsActive = PublishingLocation > 0;
            Area.IsInternalOnly = PublishingLocation == 1;
            AreaSettings.AllowPeople = ProfileInformation > 0;
            AreaSettings.AllowAdministratorsAccessToPeople = ProfileInformation > 1;
            AreaSettings.AllowInformationForIllinoisExpertsMembers = ProfileInformation > 2;
            _ = await AreaHelper.UpdateArea(Area, await AuthenticationStateProvider.GetUser());
            _ = await AreaHelper.UpdateAreaSettings(AreaSettings, Area.Title, await AuthenticationStateProvider.GetUser());
            if (_originalAllowAccess != AreaSettings.AllowAdministratorsAccessToPeople) {
                _ = await SecurityEntryHelper.SetAccessToOtherPeople(Area.Id, AreaSettings.AllowAdministratorsAccessToPeople);
                _originalAllowAccess = AreaSettings.AllowAdministratorsAccessToPeople;
            }
            if (_originalUrlProfile != AreaSettings.UrlProfile) {
                _ = await EmployeeHelper.UpdateAllEmployeeUrlProfiles(Area.Id, AreaSettings.UrlProfile);
                _originalUrlProfile = AreaSettings.UrlProfile;
            }
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Internal settings updated");
            _isDirty = false;
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var cachedAreaThinObject = CacheHelper.GetCachedArea(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder);
            if (cachedAreaThinObject != null) {
                UnitId = cachedAreaThinObject.Id;
                UnitTitle = cachedAreaThinObject.Title;
                await AssignTextFields();
            }
            _areaThinObjects = await AccessHelper.GetAreas(await AuthenticationStateProvider.GetAuthenticationStateAsync(), PersonOptionHelper);
            if (_areaThinObjects.IsSingle()) {
                UnitId = _areaThinObjects.Single().Id;
                UnitTitle = _areaThinObjects.Single().Title;
                await AssignTextFields();
            }
        }

        protected void SetDirty() => _isDirty = true;

        private static int SetProfileInformation(AreaSettings areaSettings) {
            if (!areaSettings.AllowPeople) {
                return 0;
            }
            if (!areaSettings.AllowAdministratorsAccessToPeople) {
                return 1;
            }
            if (!areaSettings.AllowInformationForIllinoisExpertsMembers) {
                return 2;
            }
            return 3;
        }

        private static int SetPublishingLocation(Area area) {
            if (!area.IsActive) {
                return 0;
            }
            if (area.IsInternalOnly) {
                return 1;
            }
            return 2;
        }

        private async Task AssignTextFields() {
            Area = await AreaHelper.GetAreaById(UnitId, await AuthenticationStateProvider.GetUser());
            AreaSettings = await AreaHelper.GetAreaSettingsByAreaId(Area.Id);
            PublishingLocation = SetPublishingLocation(Area);
            ProfileInformation = SetProfileInformation(AreaSettings);
            _originalAllowAccess = AreaSettings.AllowAdministratorsAccessToPeople;
            _originalUrlProfile = AreaSettings.UrlProfile;
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