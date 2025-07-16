using Azure.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_external.DataWarehouse;

namespace uofi_itp_directory.Pages.Admin {

    public partial class AddNewUnit {
        public List<Area> Areas { get; set; } = default!;

        public string Name { get; set; } = "";
        public string NetId { get; set; } = "";
        public string UnitName { get; set; } = "";

        [Inject]
        protected AreaHelper AreaHelper { get; set; } = default!;

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

        public async Task LookupId() {
            var name = await DataWarehouseManager.GetDataWarehouseItem(NetId);
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", string.IsNullOrWhiteSpace(name.Name) ? "No name found" : name.Name);
        }

        public async Task RemoveEntry(int id) {
            var numberItemsDeleted = await AreaHelper.RemoveArea(Areas.Single(a => a.Id == id), await AuthenticationStateProvider.GetUser());
            if (numberItemsDeleted != 0) {
                Areas.RemoveAll(a => a.Id == id);
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Area deleted");
            } else {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Offices were in the list");
            }
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task Send() {
            if (!string.IsNullOrWhiteSpace(NetId) && !string.IsNullOrWhiteSpace(UnitName)) {
                var (message, newArea) = await AreaHelper.GenerateArea(UnitName, NetId, await AuthenticationStateProvider.GetUser());
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", message);
                if (newArea != null) {
                    Areas.Add(newArea);
                }
                NetId = "";
                UnitName = "";
                StateHasChanged();
            } else {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Name and ID are required");
            }
        }

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var authstate = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var name = authstate.User?.Identity?.Name;
            if (!await PersonOptionHelper.IsFullAdmin(name)) {
                throw new AuthenticationFailedException("Full Admin access required");
            }
            Areas = await AreaHelper.GetAreas();
        }
    }
}