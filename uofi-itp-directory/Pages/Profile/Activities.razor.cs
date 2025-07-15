using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Helpers;
using uofi_itp_directory_external.Experts;

namespace uofi_itp_directory.Pages.Profile {

    public partial class Activities {
        private bool _isDirty = false;
        public Employee? Employee { get; set; } = default!;

        [CascadingParameter]
        public LayoutProfile Layout { get; set; } = default!;
        public string Instructions { get; set; } = "";

        [Parameter]
        public string Refresh { get; set; } = "";

        public bool ShouldUseExperts { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected DirectoryHookHelper DirectoryHookHelper { get; set; } = default!;

        [Inject]
        protected EmployeeActivityHelper EmployeeActivityHelper { get; set; } = default!;

        [Inject]
        protected EmployeeAreaHelper EmployeeAreaHelper { get; set; } = default!;

        [Inject]
        protected EmployeeHelper EmployeeSecurityHelper { get; set; } = default!;

        [Inject]
        protected IllinoisExpertsManager IllinoisExpertsManager { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        public async Task Delete(EmployeeActivity activity) {
            if (await JsRuntime.InvokeAsync<bool>("confirm", $"This will delete the activity \"{activity.Title}\". Are you sure?")) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Activity deleted");
                _ = await EmployeeActivityHelper.DeleteActivity(activity, Employee?.Id ?? 0, Employee?.NetId ?? "", await AuthenticationStateProvider.GetUser());
                _isDirty = false;
            }
        }

        public void New() {
            if (Employee != null) {
                Employee.EmployeeActivities.Add(new() { InEditState = true });
                _isDirty = true;
            }
        }

        public async Task RefreshDirectory() {
            if (Employee == null) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "No employee to refresh");
                return;
            }
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"Directory Entry Starting Refresh");
            var results = await DirectoryHookHelper.SendHook(Employee.Id, false);
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", results.isSuccessful ? "Directory Entry refresh complete" : results.results);
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Save(EmployeeActivity activity) {
            if (activity.InEditState) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"Activity \"{activity.Title}\" updated");
                _ = await EmployeeActivityHelper.SaveActivity(activity, Employee?.Id ?? 0, Employee?.NetId ?? "", await AuthenticationStateProvider.GetUser());
                activity.InEditState = false;
                _isDirty = false;
            } else {
                activity.InEditState = true;
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var employeeId = CacheHelper.GetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, Refresh);
            Employee = await AccessHelper.GetEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), EmployeeSecurityHelper, employeeId);
            if (Employee == null) {
                throw new Exception("No employee");
            } else {
                ShouldUseExperts = await EmployeeAreaHelper.ShouldUseExperts(Employee.NetId) && await IllinoisExpertsManager.IsInExperts(Employee.NetIdTruncated);
                Instructions = await EmployeeAreaHelper.ActivitiesInstructions(Employee.NetId);
                foreach (var activity in Employee.EmployeeActivities) {
                    activity.InEditState = false;
                }
            }
            StateHasChanged();
        }

        protected override async Task OnParametersSetAsync() => await OnInitializedAsync();

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