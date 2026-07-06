using System.Text;

namespace uofi_itp_directory_search.SearchStax {
    public class SearchStaxLoader {
        public SearchStaxLoader(string? url, string? apiToken) {
            this.apiToken = apiToken ?? "";
            this.url = url ?? "";
        }
        private string apiToken;
        private string url;

        public async Task<string> Send(string s) {
            using var client = new HttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url + "?commit=true");
            requestMessage.Content = new StringContent(s, Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", apiToken);
            var response = await client.SendAsync(requestMessage);
            return await response.Content?.ReadAsStringAsync() ?? "";
        }

    }
}
