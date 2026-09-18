using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory.Controls;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Orgchart;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.Pages.Offices {
    public partial class Chart {
        private readonly int _maxAllowedSize = 2000;

        private List<AreaOfficeThinObject> _areaThinObjects = default!;
        private bool _isDirty = false;
        private MultiChoice? _multiChoice = default!;

        private List<AreaOfficeThinObject> _officeThinObjects = default!;

        [CascadingParameter]
        public LayoutOffice Layout { get; set; } = default!;

        public Office Office { get; set; } = default!;

        public int? OfficeId { get; set; }

        public string OfficeTitle { get; set; } = "Office";

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected OfficeHelper OfficeHelper { get; set; } = default!;

        [Inject]
        protected PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        [Inject]
        protected SecurityEntryHelper SecurityEntryHelper { get; set; } = default!;

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task AssignId() {
            OfficeId = _multiChoice?.SelectedId;
            OfficeTitle = _multiChoice?.SelectedTitle ?? "";
            await AssignTextFields();
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

        public async Task<bool> RemoveChart() {
            var results = await OfficeHelper.UpdateOfficeChart(OfficeId ?? 0, "", "", await AuthenticationStateProvider.GetUser());
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Chart removed successfully");
            return true;
        }
        public async Task<bool> UploadFile(InputFileChangeEventArgs e) {
            if (e.File.Size > 1024 * _maxAllowedSize) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"File is too large -- size of file is {float.Round(e.File.Size / (float)(1024 * 1000), 2)}MB and maximum size is {_maxAllowedSize / 1000}MB");
                return false;
            }
            var stream = e.File.OpenReadStream(maxAllowedSize: 1024 * _maxAllowedSize);
            using var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();
            var json = ChartConverter.ConvertToJson(text);
            if (json.StartsWith("{")) {
                var results = await OfficeHelper.UpdateOfficeChart(OfficeId ?? 0, text, json, await AuthenticationStateProvider.GetUser());
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "File uploaded successfully");
            } else {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Error: " + json);
            }
            return true;
        }

        protected void SetDirty() => _isDirty = true;

        private async Task AssignTextFields() {
            if (OfficeId.HasValue) {
                Office = await OfficeHelper.GetOfficeById(OfficeId.Value, await AuthenticationStateProvider.GetUser());
            }
        }

        private async Task LocationChangingHandler(LocationChangingContext arg) {
            if (_isDirty) {
                if (!(await JsRuntime.InvokeAsync<bool>("confirm", $"You have unsaved changes. Are you sure?"))) {
                    arg.PreventNavigation();
                }
            }
        }
    }
}
