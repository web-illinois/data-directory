using System.Text;

namespace uofi_itp_directory_search.SearchStax {
    public class SearchStaxLoader {
        public SearchStaxLoader(string? url, string? apiToken) {
            this.apiToken = apiToken ?? "";
            this.url = string.IsNullOrWhiteSpace(url) ? "" : url.TrimEnd('/');
        }
        private string apiToken;
        private string url;
        public bool IsValid => !string.IsNullOrEmpty(apiToken) && !string.IsNullOrEmpty(url);

        public async Task<string> Send(string s) {
            if (!IsValid) {
                return "SearchStax not set up";
            }
            using var client = new HttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url + "/update?commit=true");
            requestMessage.Content = new StringContent(s, Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", apiToken);
            var response = await client.SendAsync(requestMessage);
            return await response.Content?.ReadAsStringAsync() ?? "";
        }
    }
}
