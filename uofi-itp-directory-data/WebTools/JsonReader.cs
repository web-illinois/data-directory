using System.Text.Json.Nodes;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.WebTools {
    public class JsonReader {
        private const string _baseUrl = "https://directory.illinois.edu/";
        private string key = "";

        private Dictionary<ToolkitTypeEnum, string> path = new() {
            { ToolkitTypeEnum.Person, "person" },
            { ToolkitTypeEnum.Department, "department" },
            { ToolkitTypeEnum.Service, "service" }
        };

        private Dictionary<ToolkitTypeEnum, string> nodeName = new() {
            { ToolkitTypeEnum.Person, "persons" },
            { ToolkitTypeEnum.Department, "departments" },
            { ToolkitTypeEnum.Service, "services" }
        };

        private Dictionary<ToolkitTypeEnum, string> schemaPath = new() {
            { ToolkitTypeEnum.Person, "person.schema.json" },
            { ToolkitTypeEnum.Department, "department.schema.json" },
            { ToolkitTypeEnum.Service, "service.schema.json" }
        };

        public JsonReader(string key) {
            this.key = key;
        }

        public async Task<(bool, List<string>)> GetJson(ToolkitTypeEnum toolkitType, int pageNumber, string timestamp) {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}{path[toolkitType]}?key={key}&page={pageNumber}&timestamp={timestamp}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var root = JsonNode.Parse(jsonString);
            var results = new List<string>();
            var isMore = root?["hasMore"]?.GetValue<bool>() ?? false;
            if (root?[nodeName[toolkitType]] is JsonArray items) {
                foreach (var item in items) {
                    if (item != null) {
                        results.Add(item.ToJsonString());
                    }
                }
            }
            return (isMore, results);
        }

        public async Task<string> GetSchema(ToolkitTypeEnum toolkitType) {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}{schemaPath[toolkitType]}");
            return await response.Content.ReadAsStringAsync();
        }
    }
}
