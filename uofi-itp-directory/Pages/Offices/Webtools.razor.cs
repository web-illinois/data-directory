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
using uofi_itp_directory_data.WebTools;

namespace uofi_itp_directory.Pages.Offices {
    public partial class Webtools {
        private List<AreaOfficeThinObject> _areaThinObjects = default!;
        private bool _isDirty = false;
        private MultiChoice? _multiChoice = default!;

        private List<AreaOfficeThinObject> _officeThinObjects = default!;

        public string DepartmentCodeLookup { get; set; } = "";

        public List<KeyValuePair<string, string>> DepartmentCodes { get; set; } = default!;
        public string ServiceCodeLookup { get; set; } = "";

        public List<KeyValuePair<string, string>> ServiceCodes { get; set; } = default!;

        public int AutomaticImport { get; set; } = 0;

        [CascadingParameter]
        public LayoutOffice Layout { get; set; } = default!;

        public Office Office { get; set; } = default!;

        public int? OfficeId { get; set; }

        public OfficeSettings OfficeSettings { get; set; } = default!;

        public string OfficeTitle { get; set; } = "Office";

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected WebtoolsImporter WebtoolsImporter { get; set; } = default!;

        [Inject]
        protected WebtoolsLookup WebtoolsLookup { get; set; } = default!;

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
        public void SetCall() {
            if (!string.IsNullOrWhiteSpace(DepartmentCodeLookup) && Office != null) {
                Office.DepartmentId = DepartmentCodeLookup;
                Office.ServiceId = "";
            }
            if (!string.IsNullOrWhiteSpace(ServiceCodeLookup) && Office != null) {
                Office.DepartmentId = "";
                Office.ServiceId = ServiceCodeLookup;
            }
            DepartmentCodeLookup = "";
            ServiceCodeLookup = "";
        }
        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            OfficeSettings.WebToolsAutomaticEmployeeImport = AutomaticImport == 2;
            OfficeSettings.WebToolsAutomaticImport = AutomaticImport != 0;
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Starting to load information - please be patient");
            _ = OfficeHelper.UpdateOffice(Office, await AuthenticationStateProvider.GetUser());
            _ = OfficeHelper.UpdateOfficeSettings(OfficeSettings, Office.Title, await AuthenticationStateProvider.GetUser());
            var results = "Information updated";
            if (!string.IsNullOrWhiteSpace(Office.DepartmentId)) {
                results = await WebtoolsImporter.ImportDepartment(Office, await AuthenticationStateProvider.GetUser());
            } else if (!string.IsNullOrWhiteSpace(Office.ServiceId)) {
                results = await WebtoolsImporter.ImportService(Office, await AuthenticationStateProvider.GetUser());
            }
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", results);
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
            } else {
                _areaThinObjects = await AccessHelper.GetAreas(await AuthenticationStateProvider.GetAuthenticationStateAsync(), PersonOptionHelper);
            }
        }

        protected void SetDirty() => _isDirty = true;

        private async Task AssignTextFields() {
            if (OfficeId.HasValue) {
                Office = await OfficeHelper.GetOfficeById(OfficeId.Value, await AuthenticationStateProvider.GetUser());
                OfficeSettings = await OfficeHelper.GetOfficeSettingsById(Office.Id);
                DepartmentCodes = await WebtoolsLookup.GetDepartmentCodes();
                ServiceCodes = await WebtoolsLookup.GetServiceCodes();
                if (OfficeSettings.WebToolsAutomaticEmployeeImport) {
                    AutomaticImport = 2;
                } else if (OfficeSettings.WebToolsAutomaticImport) {
                    AutomaticImport = 1;
                } else {
                    AutomaticImport = 0;
                }
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
