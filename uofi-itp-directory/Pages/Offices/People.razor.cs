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

    public partial class People {
        private List<AreaOfficeThinObject> _areaThinObjects = default!;
        private MultiChoice? _multiChoice = default!;
        private List<AreaOfficeThinObject> _officeThinObjects = default!;
        public bool IsPersonDisabled => SelectedProfile == 0;
        public List<JobProfileThinObject> JobProfiles { get; set; } = default!;

        [CascadingParameter]
        public LayoutOffice Layout { get; set; } = default!;

        public string NewNetId { get; set; } = string.Empty;
        public Office Office { get; set; } = default!;

        public int? OfficeId { get; set; }

        public string OfficeTitle { get; set; } = "Office";
        public int SelectedProfile { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected DataWarehouseManager DataWarehouseManager { get; set; } = default!;

        [Inject]
        protected JobProfileHelper JobProfileHelper { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected NavigationManager NavManager { get; set; } = default!;

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

        public async Task Edit() {
            CacheHelper.SetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, GetThinObject().EmployeeId);
            NavManager.NavigateTo($"/profile/general");
        }

        public async Task Remove() {
            var thinObject = GetThinObject();
            if (await JsRuntime.InvokeAsync<bool>("confirm", $"This will remove the employee {thinObject.Display} from the profile list. Are you sure?")) {
                var returnValue = await JobProfileHelper.RemoveJobProfile(thinObject.JobProfileId, thinObject.EmployeeId, thinObject.EmployeeNetId, await AuthenticationStateProvider.GetUser());
                if (returnValue == 0) {
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"Error: user {thinObject.Display} not removed - id {thinObject.JobProfileId}");
                } else {
                    JobProfiles.RemoveAll(jp => jp.JobProfileId == thinObject.JobProfileId);
                    SelectedProfile = 0;
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"User {thinObject.Display} removed");
                }
                StateHasChanged();
            }
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            if (OfficeId.HasValue && !string.IsNullOrWhiteSpace(NewNetId)) {
                var (employeeId, message) = await JobProfileHelper.GenerateJobProfile(OfficeId.Value, NewNetId, await AuthenticationStateProvider.GetUser());
                if (employeeId == 0) {
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", message);
                } else {
                    CacheHelper.SetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, employeeId);
                    NavManager.NavigateTo($"/profile/job?back=add");
                }
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
            } else {
                _areaThinObjects = await AccessHelper.GetAreas(await AuthenticationStateProvider.GetAuthenticationStateAsync(), PersonOptionHelper);
            }
        }

        private async Task AssignTextFields() {
            if (OfficeId.HasValue) {
                Office = await OfficeHelper.GetOfficeById(OfficeId.Value, await AuthenticationStateProvider.GetUser());
                JobProfiles = (await JobProfileHelper.GetJobProfileThinObjects(Office.Id)) ?? [];
            }
        }

        private JobProfileThinObject GetThinObject() => JobProfiles.Single(jp => jp.JobProfileId == SelectedProfile);
    }
}