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
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.Pages.Offices {

    public partial class Hours {
        private List<AreaOfficeThinObject> _areaThinObjects = default!;
        private bool _isDirty = false;
        private MultiChoice? _multiChoice = default!;

        private List<AreaOfficeThinObject> _officeThinObjects = default!;
        public List<DateTime?> EndTime { get; set; } = default!;

        [CascadingParameter]
        public LayoutOffice Layout { get; set; } = default!;

        public Office Office { get; set; } = default!;

        public List<OfficeHour> OfficeHours { get; set; } = default!;

        public int? OfficeId { get; set; }

        public string OfficeTitle { get; set; } = "Office";

        public List<DateTime?> StartTime { get; set; } = default!;

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

        public async Task AssignId() {
            OfficeId = _multiChoice?.SelectedId;
            OfficeTitle = _multiChoice?.SelectedTitle ?? "";
            await AssignTextFields();
        }

        public async Task Generate() {
            if (Office != null) {
                if (OfficeHours.Any(oh => oh.IsInvalid)) {
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Start Hours must not be after End Hours");
                } else {
                    Office.OfficeHourText = HourParser.GetOfficeHourString([.. OfficeHours], Office.HoursTextOverride, Office.HoursIncludeHolidayMessage);
                    foreach (var officeHour in OfficeHours) {
                        _ = await OfficeHelper.UpdateOfficeHour(officeHour, Office.Title, await AuthenticationStateProvider.GetUser());
                    }
                    _ = await OfficeHelper.UpdateOffice(Office, await AuthenticationStateProvider.GetUser());
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Text Rebuilt and Information updated");
                    _isDirty = false;
                }
            }
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

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

        protected void SetDirty() => _isDirty = true;

        private async Task AssignTextFields() {
            if (OfficeId.HasValue) {
                Office = await OfficeHelper.GetOfficeById(OfficeId.Value, await AuthenticationStateProvider.GetUser());
                OfficeHours = await OfficeHelper.GetOfficeHoursById(Office.Id);
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