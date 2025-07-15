using Blazored.TextEditor;
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

    public partial class Biography {
        private BlazoredTextEditor? QuillBiography = default!;

        public string BiographyText { get; set; } = "";

        public Employee? Employee { get; set; } = default!;
        [CascadingParameter]
        public LayoutProfile Layout { get; set; } = default!;

        public bool HideQuillInformationForUpdates { get; set; } = false;

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
        protected EmployeeAreaHelper EmployeeAreaHelper { get; set; } = default!;

        [Inject]
        protected EmployeeHelper EmployeeSecurityHelper { get; set; } = default!;

        [Inject]
        protected IllinoisExpertsManager IllinoisExpertsManager { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

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

        public async Task Send() {
            if (Employee != null && QuillBiography != null) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information starting to update");
                Employee.Biography = ConvertNestedLinks(await QuillBiography.GetHTML());
                _ = await EmployeeSecurityHelper.SaveEmployee(Employee, await AuthenticationStateProvider.GetUser(), "Biography: " + Employee.Biography);
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Information updated");
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var employeeId = CacheHelper.GetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, Refresh);
            Employee = await AccessHelper.GetEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), EmployeeSecurityHelper, employeeId);
            if (Employee == null) {
                throw new Exception("No employee");
            } else {
                HideQuillInformationForUpdates = true;
                BiographyText = Employee.Biography;
                HideQuillInformationForUpdates = false;
                ShouldUseExperts = await EmployeeAreaHelper.ShouldUseExperts(Employee.NetId) && await IllinoisExpertsManager.IsInExperts(Employee.NetIdTruncated);
            }
        }

        protected override async Task OnParametersSetAsync() => await OnInitializedAsync();

        private string ConvertNestedLinks(string html) {
            if (!html.Contains("<li class=\"ql-indent-1\">")) {
                return html;
            }
            var locationStart = 0;
            var locationEnd = 0;

            while (locationStart != -1) {
                locationStart = html.IndexOf("<li class=\"ql-indent-1\">", locationEnd);
                if (locationStart != -1) {
                    locationEnd = html.IndexOf("</li><li>", locationStart);
                    if (locationEnd == -1) {
                        locationEnd = html.IndexOf("</li></ul>", locationStart);
                    }
                    if (locationEnd == -1) {
                        locationEnd = html.IndexOf("</li></ol>", locationStart);
                    }
                    if (locationEnd != -1) {
                        html = html.Insert(locationEnd, "</li></ul>"); // add ending html
                        html = html.Remove(locationStart - 5, 5); // remove last </li>, 5 characters
                        html = html.Insert(locationStart - 5, "<ul>"); // add <ul> where the last </li> was
                    }
                }
            }
            return html.Replace(" class=\"ql-indent-1\"", "");
        }

        private async Task LocationChangingHandler(LocationChangingContext arg) {
            if (QuillBiography != null) {
                var html = await QuillBiography.GetHTML();
                if (!string.IsNullOrWhiteSpace(html) && html != Employee?.Biography) {
                    if (!(await JsRuntime.InvokeAsync<bool>("confirm", $"You have unsaved changes. Are you sure?"))) {
                        arg.PreventNavigation();
                    }
                }
            }
        }
    }
}