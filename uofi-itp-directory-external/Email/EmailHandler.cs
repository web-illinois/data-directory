using Newtonsoft.Json;
using System.Text;

namespace uofi_itp_directory_external.Email {

    public class EmailHandler(string? socketLabApiKey) {
        private const string _from = "no-reply@itpartners.illinois.edu";
        private const string _serverId = "47862";
        private const string _url = "https://api.socketlabs.com/v2/servers/47862/credentials/injection-api";

        private readonly string _socketLabApiKey = socketLabApiKey ?? "";

        public async Task<string> Send(string email, bool isDeleted) {
            var subject = isDeleted ? "Automated message -- profile deletion" : "Automated message -- profile change";
            var bodyText = isDeleted ? $"The profile '{email}' has been deleted within the last 24 hours. This may be because your account has been removed from campus records, or just removed from a directory listing. " : $"The profile '{email}' has been changed within the last 24 hours. Please verify that the profile is what you want it to be. ";
            bodyText += "You are subscribed to any changes to this profile, either because this is your profile or because you maintain an office that has this profile. Please go to https://directory.wigg.illinois.edu to verify your information, contact your office administrator, and change your preferences to no longer get these emails.";

            var (apiKey, url, error) = await GetAuthentication();
            if (!string.IsNullOrWhiteSpace(error)) {
                return error;
            }
            var json = "{\"serverId\": " + _serverId + ", \"APIKey\": \"" + apiKey + "\", \"Messages\": [ { \"To\": [ \"" + email + "\" ], \"From\": { \"emailAddress\": \"" + _from + "\" },\"Subject\": \"" + subject + "\", \"TextBody\": \"" + bodyText + "\", \"HtmlBody\": \"" + bodyText + "\" } ] }";

            using var client = new HttpClient();
            using var message = new HttpRequestMessage(HttpMethod.Post, url);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
            message.Headers.Add("Accept", "application/json");
            var task = await client.SendAsync(message);
            return task.IsSuccessStatusCode ? string.Empty : await task.Content.ReadAsStringAsync() + ": " + json;
        }

        private async Task<(string apiKey, string url, string error)> GetAuthentication() {
            using var client = new HttpClient();
            using var message = new HttpRequestMessage(HttpMethod.Get, _url);
            message.Headers.Add("Authorization", "Bearer " + _socketLabApiKey);
            message.Headers.Add("Accept", "application/json");
            var task = await client.SendAsync(message);
            if (!task.IsSuccessStatusCode) {
                return ("", "", $"no authorization code - {task.StatusCode} - {await task.Content.ReadAsStringAsync()}");
            }
            dynamic? authentication = JsonConvert.DeserializeObject(await task.Content.ReadAsStringAsync());
            return (authentication?.data.apiKey.ToString() ?? "", authentication?.data.gateway.ToString() ?? "", "");
        }
    }
}