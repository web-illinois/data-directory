using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory.Controls;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.Pages.Unit {

    public partial class PeopleManagement {
        private List<AreaOfficeThinObject> _areaThinObjects = default!;

        private MultiChoice? _multiChoice = default!;

        [CascadingParameter]
        public LayoutUnit Layout { get; set; } = default!;

        public string Message { get; set; } = "";

        public string NetId { get; set; } = "";

        public int? UnitId { get; set; }

        public string UnitTitle { get; set; } = "Unit";

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected IConfiguration Configuration { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected AreaHelper AreaHelper { get; set; } = default!;

        [Inject]
        protected PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        public void AssignId() {
            UnitId = _multiChoice?.SelectedId;
            UnitTitle = _multiChoice?.SelectedTitle ?? "";
        }

        public async Task Send() {
            var areaSettings = await AreaHelper.GetAreaSettingsByAreaId(UnitId ?? 0);
            var internalCode = areaSettings.InternalCode;
            var baseUrl = "https://directoryapi.wigg.illinois.edu/api/Load/Person/{source}/{netid}";
            var netId = NetId.Trim().Replace("@illinois.edu", "", StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(netId) || string.IsNullOrWhiteSpace(internalCode)) {
                Message = "NetID, directory API URL, or Internal Code is missing.";
                return;
            }
            var url = baseUrl.Replace("{netid}", Uri.EscapeDataString(netId)).Replace("{source}", Uri.EscapeDataString(internalCode));
            try {
                using var client = new HttpClient();
                using var response = await client.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                Message = response.IsSuccessStatusCode ? $"{netId} sent for {UnitTitle}. {responseContent}" : $"Directory API returned {(int)response.StatusCode}.";
            } catch (HttpRequestException ex) {
                Message = $"Could not reach the directory API: {ex.Message}";
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var cached = CacheHelper.GetCachedArea(state, CacheHolder);
            if (cached != null) {
                UnitId = cached.Id;
                UnitTitle = cached.Title;
            }
            _areaThinObjects = await AccessHelper.GetAreas(state, PersonOptionHelper);
            if (_areaThinObjects.IsSingle()) {
                UnitId = _areaThinObjects.Single().Id;
                UnitTitle = _areaThinObjects.Single().Title;
            }
        }
    }
}
