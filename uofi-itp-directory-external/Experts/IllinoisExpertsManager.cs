using Newtonsoft.Json;

namespace uofi_itp_directory_external.Experts {

    public class IllinoisExpertsManager(string? baseUrl, string? key) {
        private readonly string _baseUrl = baseUrl?.Trim('/') + "/";
        private readonly string _key = key ?? "";

        public async Task<ExpertsProfile> GetExperts(string netId) {
            var profile = await new ExpertsProfile { NetId = netId }.AddPersonInformation(_baseUrl, _key);
            if (!profile.UseExperts) {
                return profile;
            }
            if (!profile.Keywords.Any()) {
                profile.Keywords = await KeywordReader.AddKeywords(profile.ExpertsId, _baseUrl, _key);
            }
            profile.Publications = await PublicationReader.AddPublications(profile.ExpertsId, _baseUrl, _key);

            var (backgrounds, presentations) = await ActivityReader.AddActivitiesToBackgrounds(profile.ExpertsId, _baseUrl, _key);
            profile.Organizations.AddRange(backgrounds);
            profile.Presentations.AddRange(presentations);
            if (!profile.Awards.Any()) {
                profile.Awards = await PrizeReader.AddPrizesToAwardsIfNotPresent(profile.ExpertsId, _baseUrl, _key);
            }
            profile.Clippings = await ClippingReader.AddClippings(profile.ExpertsId, _baseUrl, _key);
            return profile;
        }

        public async Task<bool> IsInExperts(string netid) {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            using var res = client.GetAsync($"{_baseUrl}persons?q={netid}@illinois.edu&apiKey={_key}").Result;
            if (res.StatusCode != System.Net.HttpStatusCode.OK) {
                return false;
            }
            using var content = res.Content;
            dynamic experts = await content.ReadAsStringAsync();

            var item = JsonConvert.DeserializeObject(experts);
            if (item.count > 0) {
                IEnumerable<dynamic> list = item.items;
                return list.Any(i => i.externalId.ToString() == netid + "@illinois.edu");
            }
            return false;
        }
    }
}