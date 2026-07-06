using System.Text.Json;

namespace uofi_itp_directory_search.SearchStax {
    public class JsonNamingPolicyLowerCase : JsonNamingPolicy {

        public override string ConvertName(string name) => name.ToLower();
    }
}
