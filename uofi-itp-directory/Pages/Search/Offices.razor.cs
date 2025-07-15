using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Helpers;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.Pages.Search {

    public partial class Offices {
        public bool IsEditDisabled => LookupId == null;
        public List<string> JobProfiles { get; set; } = default!;

        [Inject]
        public LookupHelper LookupHelper { get; set; } = default!;

        public int? LookupId { get; set; }
        public List<LookupThinObject> LookupThinObjects { get; set; } = [];

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [CascadingParameter]
        public LayoutSearch Layout { get; set; } = default!;

        public Office? Office { get; set; } = null!;
        public List<OfficeManager> OfficeManagers { get; set; } = default!;

        [Inject]
        public PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected JobProfileHelper JobProfileHelper { get; set; } = default!;

        [Inject]
        protected OfficeHelper OfficeHelper { get; set; } = default!;

        [Inject]
        protected OfficeManagerHelper OfficeManagerHelper { get; set; } = default!;

        public void ClearOffice() {
            Office = null;
        }

        public async Task EditOffice() {
            if (LookupId != null) {
                Office = await OfficeHelper.GetOfficeById(LookupId.Value, await AuthenticationStateProvider.GetUser());
                if (await PersonOptionHelper.IsOfficeAdmin(await AuthenticationStateProvider.GetUser(), Office.Id)) {
                    var title = LookupThinObjects.First(lto => lto.Id == LookupId.Value).Text;
                    CacheHelper.SetCachedOffice(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, new AreaOfficeThinObject(LookupId.Value, title));
                    ClearOffice();
                    NavigationManager.NavigateTo("/office/general?back=search");
                } else {
                    Office.OfficeSettings = await OfficeHelper.GetOfficeSettingsById(LookupId.Value);
                    Office.OfficeHours = await OfficeHelper.GetOfficeHoursById(LookupId.Value);
                    OfficeManagers = await OfficeManagerHelper.GetOfficeManagersById(Office.Id);
                    JobProfiles = (await JobProfileHelper.GetJobProfileThinObjects(Office.Id)).Select(j => j.Display).ToList();
                }
            }
        }

        protected void ChangeLookupId(ChangeEventArgs e) => LookupId = int.TryParse(e.Value?.ToString(), out var lookupId) ? lookupId : null;

        protected async Task FilterChange(ChangeEventArgs e) {
            LookupThinObjects = await LookupHelper.GetOffices(e.Value?.ToString() ?? "");
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            LookupThinObjects = await LookupHelper.GetOffices("");
        }
    }
}