using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory.Controls;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Helpers;

namespace uofi_itp_directory.Pages.Profile {

    public partial class Audio {
        private bool _isDirty = false;
        public AudioUploader? AudioUploader { get; set; } = default!;
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

        public void DeleteAudio() {
            if (Employee != null && AudioUploader != null) {
                Employee.AudioUrl = "";
                _isDirty = true;
                StateHasChanged();
            }
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public void SaveAudio() {
            if (Employee != null && AudioUploader != null) {
                Employee.AudioUrl = AudioUploader.FileUrl;
                _isDirty = true;
                StateHasChanged();
            }
        }

        public async Task Send() {
            if (Employee != null && AudioUploader != null) {
                if (await AudioUploader.SaveFile()) {
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information starting to update");
                    Employee.AudioUrl = AudioUploader.FileUrl;
                    _ = await EmployeeSecurityHelper.SaveEmployee(Employee, await AuthenticationStateProvider.GetUser(), "Audio Updated");
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information updated");
                    _isDirty = false;
                }
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var employeeId = CacheHelper.GetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, Refresh);
            Employee = await AccessHelper.GetEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), EmployeeSecurityHelper, employeeId);
            if (Employee == null) {
                throw new Exception("No employee");
            }
        }

        protected override async Task OnParametersSetAsync() => await OnInitializedAsync();

        private async Task LocationChangingHandler(LocationChangingContext arg) {
            if (_isDirty) {
                if (!(await JsRuntime.InvokeAsync<bool>("confirm", $"You have unsaved changes. Are you sure?"))) {
                    arg.PreventNavigation();
                }
            }
        }
    }
}
