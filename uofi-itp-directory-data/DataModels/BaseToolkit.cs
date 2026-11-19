using System.Text.Json;

namespace uofi_itp_directory_data.DataModels {
    public abstract class BaseToolkit : BaseDataItem {
        public string Key { get; set; } = "";
        public string Json { get; set; } = "";
        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
