using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.Controls {

    public enum MultiChoiceTypeEnum { Area, Office }

    public partial class MultiChoice {
        public readonly Dictionary<MultiChoiceTypeEnum, string> Label = new() { { MultiChoiceTypeEnum.Area, "Choose a unit" }, { MultiChoiceTypeEnum.Office, "Choose an office" } };

        [Parameter]
        public IEnumerable<AreaOfficeThinObject> AreaOfficeParentThinObjects { get; set; } = default!;

        public IEnumerable<AreaOfficeThinObject> AreaOfficeParentThinObjectsSorted => AreaOfficeParentThinObjects.OrderBy(a => a.Title);

        [Parameter]
        public IEnumerable<AreaOfficeThinObject> AreaOfficeThinObjects { get; set; } = default!;

        public IEnumerable<AreaOfficeThinObject> AreaOfficeThinObjectsSorted() {
            if (SelectedParentId == 0) {
                return AreaOfficeThinObjects.OrderBy(a => a.Title);
            } else {
                if (SelectedId == 0) {
                    SelectedId = AreaOfficeThinObjects.Where(a => a.ParentId == SelectedParentId).OrderBy(a => a.Title).First().Id;
                }
                return AreaOfficeThinObjects.Where(a => a.ParentId == SelectedParentId).OrderBy(a => a.Title);
            }
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickCallback { get; set; }

        [Parameter]
        public int SelectedId { get; set; }

        public bool UseParentSelection() => AreaOfficeParentThinObjects != null && AreaOfficeParentThinObjects.Count() > 1;

        public int SelectedParentId { get; set; }

        [Parameter]
        public string SelectedTitle { get; set; } = "";

        [Parameter]
        public MultiChoiceTypeEnum Type { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        public async Task Click() {
            if (SelectedId != 0) {
                var selectedItem = AreaOfficeThinObjects.First(aot => aot.Id == SelectedId);
                SelectedTitle = selectedItem.Title;
                if (Type == MultiChoiceTypeEnum.Area) {
                    CacheHelper.SetCachedArea(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, selectedItem);
                } else if (Type == MultiChoiceTypeEnum.Office) {
                    CacheHelper.SetCachedOffice(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, selectedItem);
                }
                await OnClickCallback.InvokeAsync();
            }
        }
    }
}