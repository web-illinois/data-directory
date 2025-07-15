using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Helpers;

namespace uofi_itp_directory.Pages.Profile {

    public partial class Hours {
        private bool _isDirty = false;
        public Employee? Employee { get; set; } = default!;

        [CascadingParameter]
        public LayoutProfile Layout { get; set; } = default!;

        [Parameter]
        public string Refresh { get; set; } = "";

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected EmployeeHelper EmployeeSecurityHelper { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        public async Task Generate() {
            if (Employee != null) {
                if (Employee.EmployeeHours.Any(eh => eh.IsInvalid)) {
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Start Hours must be before End Hours");
                } else {
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information starting to update");
                    Employee.EmployeeHourText = HourParser.GetEmployeeHourString([.. Employee.EmployeeHours]);
                    _ = await EmployeeSecurityHelper.SaveEmployee(Employee, await AuthenticationStateProvider.GetUser(), "Hours");
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Text Rebuilt and Information updated");
                    _isDirty = false;
                }
            }
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var employeeId = CacheHelper.GetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, Refresh);
            Employee = await AccessHelper.GetEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), EmployeeSecurityHelper, employeeId);
            if (Employee == null) {
                throw new Exception("No employee");
            }
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