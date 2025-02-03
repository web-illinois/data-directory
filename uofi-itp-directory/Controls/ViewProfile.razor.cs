using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.Helpers;

namespace uofi_itp_directory.Controls {

    public partial class ViewProfile {
        public string ButtonName { get; set; } = "";
        public bool HasProfile => !string.IsNullOrWhiteSpace(Url);

        [Parameter]
        public string NetId { get; set; } = "";

        public string Url { get; set; } = "";

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected EmployeeAreaHelper EmployeeAreaHelper { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            var currentUser = await AuthenticationStateProvider.GetUser();
            Url = await EmployeeAreaHelper.ProfileViewUrlReadOnly(string.IsNullOrWhiteSpace(NetId) ? currentUser : NetId);
            ButtonName = string.IsNullOrWhiteSpace(NetId) || NetId == currentUser ? "View My Profile" : $"View Profile {NetId.Replace("@illinois.edu", "")}";
        }
    }
}