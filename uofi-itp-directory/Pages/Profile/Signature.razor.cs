using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Helpers;

namespace uofi_itp_directory.Pages.Profile {

    public partial class Signature {
        public Employee? Employee { get; set; } = default!;
        public string GenerateSignature { get; set; } = "";
        public string Instructions { get; set; } = "";

        [CascadingParameter]
        public LayoutProfile Layout { get; set; } = default!;

        [Parameter]
        public string Refresh { get; set; } = "";

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
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SignatureGenerator SignatureGenerator { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var employeeId = CacheHelper.GetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, Refresh);
            Employee = await EmployeeSecurityHelper.GetEmployeeForSignature(employeeId, await AuthenticationStateProvider.GetUser());
            var settings = await EmployeeSecurityHelper.GetEmployeeSettings(Employee);
            if (Employee == null) {
                throw new Exception("No employee");
            }
            GenerateSignature = SignatureGenerator.GenerateSignatureUrl(Employee, settings.SignatureExtension);
            Instructions = await EmployeeAreaHelper.ActivitiesInstructions(Employee.NetId);
        }

        protected override async Task OnParametersSetAsync() => await OnInitializedAsync();
    }
}