using Amazon;
using Amazon.Runtime;
using OpenSearch.Client;
using OpenSearch.Net;
using OpenSearch.Net.Auth.AwsSigV4;

namespace uofi_itp_directory_search {
    public static class LowLevelClientFactory {
        public static OpenSearchLowLevelClient GenerateClient(string? baseUrl, string? accessKey, string? secretKey) {
            var nodeAddress = new Uri(baseUrl ?? "");
            var connection = string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey) ? null : new AwsSigV4HttpConnection(new BasicAWSCredentials(accessKey, secretKey), RegionEndpoint.USEast2);
            var config = new ConnectionSettings(nodeAddress, connection);
            return new OpenSearchLowLevelClient(config);
        }

        public static string Index = "dr_person";
    }
}
