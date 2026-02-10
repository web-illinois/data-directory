using Azure.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataJson;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_external.DataWarehouse;

namespace uofi_itp_directory.Pages.Admin {
    public partial class Json {
        private const int _maxFileSize = 20480000;
        private string _reader = "";

        public string Name { get; set; } = "";
        public string NetId { get; set; } = "";
        public string UnitName { get; set; } = "";

        public string SelectedOption { get; set; } = "";

        [Inject]
        protected DirectoryRepository DirectoryRepository { get; set; } = default!;

        [Inject]
        protected AreaHelper AreaHelper { get; set; } = default!;

        [Inject]
        protected OfficeHelper OfficeHelper { get; set; } = default!;

        [Inject]
        protected EmployeeHelper EmployeeHelper { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected DataWarehouseManager DataWarehouseManager { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected PersonOptionHelper PersonOptionHelper { get; set; } = default!;
        [CascadingParameter]
        public LayoutAdmin Layout { get; set; } = default!;

        public async Task ExportJson() {
            var areas = await AreaHelper.GetAreasIncludeEverything();
            var employees = await EmployeeHelper.GetAllEmployeesIncludeEverything();
            var directory = new DirectoryJson { Employees = [.. employees.Select(e => new EmployeeJson(e))], Areas = [.. areas.Select(a => new AreaJson(a))] };
            var jsonName = $"DirectoryData_{DateTime.Now.ToString("yyyy_MM_dd")}.json";
            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(JsonSerializer.Serialize(directory)));
            using var streamRef = new DotNetStreamReference(fileStream);
            await JsRuntime.InvokeVoidAsync("downloadFileFromStream", jsonName, streamRef);
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Downloaded: " + jsonName);
        }


        private async Task LoadFile(InputFileChangeEventArgs e) {
            _reader = await new StreamReader(e.File.OpenReadStream(_maxFileSize)).ReadToEndAsync();
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"File Uploaded to System {_reader.Length} bytes -- need to click the button to import");
        }


        public async Task ImportJson() {
            var directory = Newtonsoft.Json.JsonConvert.DeserializeObject<DirectoryJson>(_reader ?? "");
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"Starting with {directory?.Employees.Count} Employees and {directory?.Areas.Count} Areas - {(directory?.DeleteAllEntries ?? false ? "Deteting existing items" : "")}");
            if (directory != null) {
                if (directory.DeleteAllEntries) {
                    var authstate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                    var name = authstate.User?.Identity?.Name;
                    await DirectoryRepository.DeleteAllEntries(name ?? "");
                }
                var employeeCount = 0;
                var areaCount = 0;
                var employees = directory?.Employees.Select(e => e.ToEmployee()).ToList() ?? [];
                foreach (var employee in employees) {
                    var employeeId = await EmployeeHelper.SaveEmployeeFromJson(employee);
                    if (employeeId > 0) {
                        employeeCount++;
                        directory?.RestoreEmployeeIdForJobProfiles(employee.NetId, employeeId);
                    }
                }
                var areas = directory?.Areas.Select(a => a.ToArea()).ToList() ?? [];
                foreach (var area in areas) {
                    if ((await AreaHelper.SaveAreaFromJson(area)) > 0) {
                        areaCount++;
                    }
                }
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"Imported {employees.Count} Employees and {areas.Count} Areas");
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var authstate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var name = authstate.User?.Identity?.Name;
            if (!await PersonOptionHelper.IsFullAdmin(name)) {
                throw new AuthenticationFailedException("Full Admin access required");
            }
        }
    }
}
