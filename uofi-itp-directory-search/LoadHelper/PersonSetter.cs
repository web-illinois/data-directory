using Newtonsoft.Json;
using OpenSearch.Net;
using uofi_itp_directory_search.ViewModel;

namespace uofi_itp_directory_search.LoadHelper {

    public class PersonSetter(string searchUrl, OpenSearchLowLevelClient? openSearchLowLevelClient, Action<string> logger) {
        private readonly Action<string> _logger = logger;
        private readonly OpenSearchLowLevelClient _openSearchLowLevelClient = openSearchLowLevelClient ?? default!;
        private readonly bool _logOnly = string.IsNullOrWhiteSpace(searchUrl);

        public async Task<bool> DeleteSingle(string source, string netid) => await SendInformation(source, netid, "");

        public async Task<bool> SaveSingle(Employee employee) => await SendInformation(employee.Source, employee.NetId, JsonConvert.SerializeObject(employee));

        private async Task<bool> SendInformation(string source, string netid, string json) {
            var isDelete = string.IsNullOrWhiteSpace(json);
            _logger($"{source} / {netid} {(isDelete ? "DELETE" : "PUT")} {(_logOnly ? json : "")}");
            if (_logOnly) {
                return true;
            } else {
                try {
                    if (isDelete) {
                        var text = await _openSearchLowLevelClient.DeleteAsync<StringResponse>(LowLevelClientFactory.Index, Employee.GenerateId(source, netid));
                    } else {
                        var text = await _openSearchLowLevelClient.IndexAsync<StringResponse>(LowLevelClientFactory.Index, Employee.GenerateId(source, netid), json);
                    }
                } catch (Exception e) {
                    _logger($"Error in {source} / {netid}: {e.Message}");
                    return false;
                }
                return true;
            }
        }
    }
}