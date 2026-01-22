using Microsoft.AspNetCore.Components;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory.Controls {
    public partial class LastUpdated {
        [Parameter]
        public Employee Employee { get; set; } = default!;

    }
}
