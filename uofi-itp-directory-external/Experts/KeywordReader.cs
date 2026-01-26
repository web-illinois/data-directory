using Newtonsoft.Json;

namespace uofi_itp_directory_external.Experts {

    public static class KeywordReader {
        private static readonly int _maximumItemsToPull = 10;

        public static async Task<List<string>> AddKeywords(string expertsId, string url, string apikey) {
            var returnValue = new List<string>();
            dynamic experts = await ReaderHelper.GetItem($"{url}persons/{expertsId}/fingerprints?size=10&apiKey={apikey}");
            var fingerprints = JsonConvert.DeserializeObject(experts);
            var items = fingerprints.items;
            foreach (var item in items) {
                var concepts = item.concepts;
                foreach (var concept in concepts) {
                    dynamic expertsConcepts = await ReaderHelper.GetItem($"{url}concepts/{concept.uuid}?apiKey={apikey}");
                    var conceptText = JsonConvert.DeserializeObject(expertsConcepts);
                    returnValue.Add(conceptText.name.text[0].value.ToString());
                    if (returnValue.Count >= _maximumItemsToPull) {
                        break;
                    }
                }
            }
            return returnValue;
        }
    }
}