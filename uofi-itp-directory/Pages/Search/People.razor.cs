using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.Helpers;

namespace uofi_itp_directory.Pages.Search {

    public partial class People {
        public bool IsEditDisabled => LookupId == null;

        [Inject]
        public LookupHelper LookupHelper { get; set; } = default!;
        [CascadingParameter]
        public LayoutSearch Layout { get; set; } = default!;

        public int? LookupId { get; set; }
        public List<LookupThinObject> LookupThinObjects { get; set; } = [];

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        public async Task EditProfile() {
            if (LookupId != null) {
                CacheHelper.SetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, LookupId);
                NavigationManager.NavigateTo("/profile/general?back=search");
            }
        }

        protected void ChangeLookupId(ChangeEventArgs e) => LookupId = int.TryParse(e.Value?.ToString(), out var lookupId) ? lookupId : null;

        protected async Task FilterChange(ChangeEventArgs e) {
            LookupThinObjects = await LookupHelper.GetJobProfiles(e.Value?.ToString() ?? "");
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            LookupThinObjects = await LookupHelper.GetJobProfiles("");
        }
    }
}