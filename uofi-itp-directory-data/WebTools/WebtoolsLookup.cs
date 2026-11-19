using System.Text.Json.Nodes;
using uofi_itp_directory_data.Data;

namespace uofi_itp_directory_data.WebTools {
    public class WebtoolsLookup(DirectoryRepository? directoryRepository) {
        private readonly DirectoryRepository _directoryRepository = directoryRepository ?? throw new ArgumentNullException("directoryRepository");

        public async Task<List<KeyValuePair<string, string>>> GetDepartmentCodes() {
            var returnValue = new Dictionary<string, string>();
            var toolkitDepartments = await _directoryRepository.ReadAsync(d => d.ToolkitDepartments);
            foreach (var toolkitDepartment in toolkitDepartments) {
                var root = JsonNode.Parse(toolkitDepartment.Json);
                var name = root?["name"]?.ToString() ?? "";
                returnValue.Add(toolkitDepartment.Key, name);
            }
            return [.. returnValue.OrderBy(pair => pair.Value)];
        }
        public async Task<List<KeyValuePair<string, string>>> GetServiceCodes() {
            var returnValue = new Dictionary<string, string>();
            var toolkitServices = await _directoryRepository.ReadAsync(d => d.ToolkitServices);
            foreach (var toolkitService in toolkitServices) {
                var root = JsonNode.Parse(toolkitService.Json);
                var name = root?["name"]?.ToString() ?? "";
                returnValue.Add(toolkitService.Key, name);
            }
            return [.. returnValue.OrderBy(pair => pair.Value)];
        }
    }
}
