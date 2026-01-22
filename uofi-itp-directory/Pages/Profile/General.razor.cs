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

    public partial class General {
        private bool _isDirty = false;

        public Employee? Employee { get; set; } = default!;
        public string Instructions { get; set; } = "";

        [CascadingParameter]
        public LayoutProfile Layout { get; set; } = default!;

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

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected SecurityEntryHelper SecurityEntryHelper { get; set; } = default!;

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            if (Employee != null) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information starting to update");
                _ = await EmployeeSecurityHelper.SaveEmployee(Employee, await AuthenticationStateProvider.GetUser(), "Employee General");
                _ = await SecurityEntryHelper.UpdateSecurityEntry(Employee.NetId, Employee.NameFirst, Employee.NameLast);
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information updated");
                _isDirty = false;
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var employeeId = CacheHelper.GetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, Refresh);
            Employee = await AccessHelper.GetEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), EmployeeSecurityHelper, employeeId);
            if (Employee == null) {
                throw new Exception("No employee");
            }
            Instructions = await EmployeeAreaHelper.EmployeeInstructions(Employee.NetId);
            PersonName = Employee.ProfileName;
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