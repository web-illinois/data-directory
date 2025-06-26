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

    public partial class Headshot {
        private bool _isDirty = false;
        public Employee? Employee { get; set; } = default!;

        public int Height { get; set; }

        public ImageUploader? ImageUploader { get; set; } = default!;

        public string Instructions { get; set; } = "";

        [Parameter]
        public string Refresh { get; set; } = "";

        public string Size { get; set; } = "";

        public int Width { get; set; }

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

        public void DeleteImage() {
            if (Employee != null && ImageUploader != null) {
                Employee.PhotoUrl = "";
                _isDirty = true;
                StateHasChanged();
            }
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public void SaveImage() {
            if (Employee != null && ImageUploader != null) {
                Employee.PhotoUrl = ImageUploader.FileUrl;
                _isDirty = true;
                StateHasChanged();
            }
        }

        public async Task Send() {
            if (Employee != null && ImageUploader != null) {
                if (await ImageUploader.SaveFile()) {
                    Employee.PhotoUrl = ImageUploader.FileUrl;
                }
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information starting to update");
                _ = await EmployeeSecurityHelper.SaveEmployee(Employee, await AuthenticationStateProvider.GetUser(), "Headshot");
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information updated");
                _isDirty = false;
            }
        }

        protected override async Task OnInitializedAsync() {
            var employeeId = CacheHelper.GetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, Refresh);
            Employee = await AccessHelper.GetEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), EmployeeSecurityHelper, employeeId);
            var areaSettings = await EmployeeSecurityHelper.GetEmployeeSettings(Employee);
            Height = areaSettings.PictureHeight;
            Width = areaSettings.PictureWidth;
            Size = Height == 0 || Width == 0 ? "" : $"({Width}w x {Height}h)";
            if (Employee == null) {
                throw new Exception("No employee");
            } else {
                Instructions = await EmployeeAreaHelper.HeadshotInstructions(Employee.NetId);
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