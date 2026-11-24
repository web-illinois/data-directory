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

    public partial class OrgChart {
        private List<AreaOfficeThinObject> _areaThinObjects = default!;
        private MultiChoice? _multiChoice = default!;
        public Area Area { get; set; } = default!;
        public AreaSettings AreaSettings { get; set; } = default!;

        [CascadingParameter]
        public LayoutUnit Layout { get; set; } = default!;

        public int? UnitId { get; set; }

        public string UnitTitle { get; set; } = "Unit";

        public List<Office> OfficeThinObjects { get; set; } = default!;

        [Inject]
        protected AreaHelper AreaHelper { get; set; } = default!;

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

        public async Task AssignId() {
            UnitId = _multiChoice?.SelectedId;
            UnitTitle = _multiChoice?.SelectedTitle ?? "";
            await AssignTextFields();
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            _ = await AreaHelper.UpdateArea(Area, await AuthenticationStateProvider.GetUser());
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Organization Chart updated");
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

        private async Task AssignTextFields() {
            Area = await AreaHelper.GetAreaById(UnitId, await AuthenticationStateProvider.GetUser());
            AreaSettings = await AreaHelper.GetAreaSettingsByAreaId(Area.Id);
            OfficeThinObjects = await OfficeHelper.GetOffices(Area.Id);
        }
    }
}