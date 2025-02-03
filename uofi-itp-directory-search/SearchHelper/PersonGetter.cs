using Newtonsoft.Json;
using OpenSearch.Net;
using uofi_itp_directory_search.ViewModel;

namespace uofi_itp_directory_search.SearchHelper {

    public class PersonGetter(OpenSearchLowLevelClient? openSearchLowLevelClient) {
        private readonly OpenSearchLowLevelClient _openSearchLowLevelClient = openSearchLowLevelClient ?? throw new ArgumentNullException("openSearchLowLevelClient");

        public async Task<Employee> GetByUin(string username, string source) => await GetEmployee(JsonStringManager.GetEmployeeJsonByUin(source, username));

        public async Task<Employee> GetByUsername(string username, string source) => await GetEmployee(JsonStringManager.GetEmployeeJsonByName(source, username));

        public async Task<IEnumerable<string>> GetSuggestions(string query, string source) {
            var searchResponse = await _openSearchLowLevelClient.SearchAsync<StringResponse>(LowLevelClientFactory.Index, JsonStringManager.GetSuggestionJson(source, query));
            dynamic? json = JsonConvert.DeserializeObject(searchResponse.Body ?? "");

            var returnValue = new List<string>();
            if (json?.suggest.suggest != null) {
                foreach (var item in json.suggest.suggest[0].options) {
                    returnValue.Add(item.text.ToString());
                }
            }
            return returnValue;
        }

        public async Task<DirectoryItem> Search(string query, IEnumerable<string> offices, IEnumerable<string> jobTypes, IEnumerable<string> tags, bool useFullText, int skip, int size, string source) {
            var searchResponse = await _openSearchLowLevelClient.SearchAsync<StringResponse>(LowLevelClientFactory.Index, JsonStringManager.GetJsonForSearch(query, skip, size, JsonStringManager.GetJsonFilter(source, offices, jobTypes, tags), useFullText));
            dynamic? json = JsonConvert.DeserializeObject(searchResponse.Body ?? "");

            var returnValue = new DirectoryItem();
            if (json != null && json?.hits.hits.Count > 0) {
                returnValue.Count = Convert.ToInt32(json?.hits.total.value);
                foreach (var hit in json?.hits.hits ?? new dynamic[0]) {
                    returnValue.People.Add(JsonConvert.DeserializeObject<Employee>(hit._source.ToString()));
                }
            }
            if (json != null && json?.suggest != null && json?.suggest.suggestion.Count > 0 && json?.suggest.suggestion[0].options.Count > 0) {
                returnValue.Suggestion = json?.suggest.suggestion[0].options[0].text.ToString() ?? "";
            }
            if (offices.Count() == 1) {
                foreach (var emp in returnValue.People) {
                    emp.JobProfiles = emp.JobProfiles.Where(j => j.Office == offices.First()).ToList();
                    emp.PrimaryOffice = offices.First();
                    emp.PrimaryTitle = emp.JobProfiles.FirstOrDefault(j => j.Office == offices.FirstOrDefault())?.Title ?? emp.PrimaryTitle;
                }
                if (string.IsNullOrWhiteSpace(query)) {
                    returnValue.People = returnValue.People.OrderBy(p => p.JobProfiles.FirstOrDefault()?.DisplayOrder).ThenBy(p => p.FullNameReversed).ToList();
                }
            }
            return returnValue;
        }

        public async Task<DirectoryItem> SearchByArea(string query, IEnumerable<string> offices, IEnumerable<string> jobTypes, IEnumerable<string> tags, bool useFullText, string source) {
            var searchResponse = await _openSearchLowLevelClient.SearchAsync<StringResponse>(LowLevelClientFactory.Index, JsonStringManager.GetJsonForAreaSearch(query, JsonStringManager.GetJsonFilter(source, offices, jobTypes, tags), useFullText));
            dynamic? json = JsonConvert.DeserializeObject(searchResponse.Body ?? "");

            var returnValue = new DirectoryItem();
            if (json != null && json?.hits.hits.Count > 0) {
                foreach (var hit in json?.hits.hits ?? new dynamic[0]) {
                    returnValue.People.Add(JsonConvert.DeserializeObject<Employee>(hit._source.ToString()));
                }
            }
            if (json != null && json?.suggest != null && json?.suggest.suggestion.Count > 0 && json?.suggest.suggestion[0].options.Count > 0) {
                returnValue.Suggestion = json?.suggest.suggestion[0].options[0].text.ToString() ?? "";
            }
            return returnValue;
        }

        private async Task<Employee> GetEmployee(string jsonString) {
            var searchResponse = await _openSearchLowLevelClient.SearchAsync<StringResponse>(LowLevelClientFactory.Index, jsonString);
            dynamic? json = JsonConvert.DeserializeObject(searchResponse.Body ?? "");
            if (json != null && json?.hits.hits.Count > 0) {
                return JsonConvert.DeserializeObject<Employee>(json?.hits.hits[0]._source.ToString());
            }
            return new Employee();
        }
    }
}