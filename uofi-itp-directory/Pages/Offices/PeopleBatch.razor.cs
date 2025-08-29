using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory.Controls;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.Pages.Offices {

    public partial class PeopleBatch {
        public MultiChoice? _multiChoice = default!;
        private List<AreaOfficeThinObject> _officeThinObjects = default!;

        [CascadingParameter]
        public LayoutOffice Layout { get; set; } = default!;

        public string NewNetIds { get; set; } = string.Empty;
        public ProfileCategoryTypeEnum NewProfileCategory { get; set; }
        public string NewTitle { get; set; } = string.Empty;
        public Office Office { get; set; } = default!;

        public int? OfficeId { get; set; }

        public string OfficeTitle { get; set; } = "Office";
        public string Results { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected JobProfileHelper JobProfileHelper { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected NavigationManager NavManager { get; set; } = default!;

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

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            Results = "";
            if (OfficeId.HasValue && !string.IsNullOrWhiteSpace(NewNetIds)) {
                foreach (var netId in NewNetIds.Split('\n').Select(n => n.Trim([' ', '\t', '\r'])).Where(s => !string.IsNullOrWhiteSpace(s))) {
                    var (employeeId, message) = await JobProfileHelper.GenerateJobProfile(OfficeId.Value, netId, await AuthenticationStateProvider.GetUser(), NewTitle.Trim(), NewProfileCategory);
                    Results += $"{message}. ";
                    StateHasChanged();
                }
            } else {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Net IDs needs to be filled out");
            }
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
            }
        }

        private async Task AssignTextFields() {
            if (OfficeId.HasValue) {
                Office = await OfficeHelper.GetOfficeById(OfficeId.Value, await AuthenticationStateProvider.GetUser());
            }
        }
    }
}