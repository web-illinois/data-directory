using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_data.WebTools;

namespace uofi_itp_directory.Pages.Search {
    public partial class WebtoolsLog {
        public List<uofi_itp_directory_data.DataModels.ToolkitLog> LogItems { get; set; } = default!;

        public string SearchText { get; set; } = string.Empty;

        public bool IsFullAdmin { get; set; } = false;

        [CascadingParameter]
        public LayoutSearch Layout { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject]
        public PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        [Inject]
        public WebtoolsLookup WebtoolsLookup { get; set; } = default!;

        public async Task RefreshInformation() {
            LogItems = await WebtoolsLookup.GetLogs(SearchText);
        }

        public async Task DeleteLogs() {
            var authstate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var name = authstate.User?.Identity?.Name;
            IsFullAdmin = await PersonOptionHelper.IsFullAdmin(name);
            if (IsFullAdmin) {
                _ = await WebtoolsLookup.DeleteLogs();
                LogItems.Clear();
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var authstate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var name = authstate.User?.Identity?.Name;
            IsFullAdmin = await PersonOptionHelper.IsFullAdmin(name);
            await RefreshInformation();
        }
    }
}
