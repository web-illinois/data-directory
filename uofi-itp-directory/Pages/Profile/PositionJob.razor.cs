namespace uofi_itp_directory.Pages.Profile {

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Components.Routing;
    using Microsoft.JSInterop;
    using uofi_itp_directory.ControlHelper;
    using uofi_itp_directory_data.Cache;
    using uofi_itp_directory_data.DataAccess;
    using uofi_itp_directory_data.DataModels;
    using uofi_itp_directory_data.Helpers;

    public partial class PositionJob {
        private bool _isDirty = false;
        public Dictionary<int, List<AreaTag>> AreaTags { get; set; } = default!;

        public Employee? Employee { get; set; } = default!;
        public string Instructions { get; set; } = "";
        public Dictionary<int, bool> OfficeDescriptionUsed { get; set; } = default!;
        public string PersonName { get; set; } = "My Profile";

        [Parameter]
        public string Refresh { get; set; } = "";

        [SupplyParameterFromQuery(Name = "back")]
        public string? ShowBackButton { get; set; }

        [Inject]
        protected AreaHelper AreaHelper { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected EmployeeAreaHelper EmployeeAreaHelper { get; set; } = default!;

        [Inject]
        protected EmployeeHelper EmployeeSecurityHelper { get; set; } = default!;

        protected bool IsMultiple => Employee?.JobProfiles.Count() > 1;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected OfficeHelper OfficeHelper { get; set; } = default!;

        protected int? PrimaryJobProfileId { get; set; }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            if (Employee != null) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information starting to update");
                _ = await EmployeeSecurityHelper.SaveEmployee(Employee, await AuthenticationStateProvider.GetUser(), "Employee Job");
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information updated");
                _isDirty = false;
            }
        }

        protected void AddTag(AreaTag tag, JobProfile profile) {
            Employee?.JobProfiles.FirstOrDefault(jp => jp.Id == profile.Id)?.AddTag(tag.Title, tag.AllowEmployeeToEdit);
            _isDirty = true;
        }

        protected override async Task OnInitializedAsync() {
            var employeeId = CacheHelper.GetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, Refresh);
            Employee = await AccessHelper.GetEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), EmployeeSecurityHelper, employeeId);
            if (Employee == null) {
                throw new Exception("No employee");
            }
            AreaTags = [];
            foreach (var areaId in Employee.JobProfiles.Select(jp => jp.Office.AreaId).Distinct()) {
                AreaTags[areaId] = await AreaHelper.GetAreaTagsByAreaId(areaId);
            }
            OfficeDescriptionUsed = [];
            foreach (var officeId in Employee.JobProfiles.Select(jp => jp.Office.Id).Distinct()) {
                OfficeDescriptionUsed[officeId] = await OfficeHelper.GetOfficeSettingJobDescriptionById(officeId);
            }
            Instructions = await EmployeeAreaHelper.EmployeeInstructions(Employee.NetId);
            PersonName = Employee.ProfileName;
        }

        protected override async Task OnParametersSetAsync() => await OnInitializedAsync();

        protected async Task RemoveTag(AreaTag tag, JobProfile profile) {
            var profileTag = profile.Tags.FirstOrDefault(p => p.Title == tag.Title);
            if (profile != null) {
                if (profileTag?.Id != 0) {
                    _ = await EmployeeSecurityHelper.RemoveTag(profileTag);
                }
                if (profileTag != null) {
                    _ = (Employee?.JobProfiles.FirstOrDefault(jp => jp.Id == profile.Id)?.Tags.Remove(profileTag));
                }
            }
            _isDirty = true;
        }

        protected void SetDirty() => _isDirty = true;

        private async Task LocationChangingHandler(LocationChangingContext arg) {
            if (_isDirty) {
                if (!(await JsRuntime.InvokeAsync<bool>("confirm", $"You have unsaved changes. Are you sure?"))) {
                    arg.PreventNavigation();
                }
            }
        }
    }
}