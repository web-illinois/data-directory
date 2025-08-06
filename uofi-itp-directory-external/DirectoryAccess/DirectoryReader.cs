namespace uofi_itp_directory_external.DirectoryAccess {

    public static class DirectoryReader {
        private const string _baseUrl = "https://directoryapi.wigg.illinois.edu/api/Directory";

        public static async Task<string> GetPersonJson(string netid, string source) {
            using var client = new HttpClient();
            using var res = await client.GetAsync(_baseUrl + $"/GetProfile/{source}/{netid}");
            if (res.StatusCode != System.Net.HttpStatusCode.OK) {
                return string.Empty;
            }
            using var content = res.Content;
            return await content.ReadAsStringAsync();
        }
    }
}