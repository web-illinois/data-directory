using Newtonsoft.Json;

namespace uofi_itp_directory_external.Experts {
    public static class ClippingReader {
        public static async Task<List<ExpertsItem>> AddClippings(string expertsId, string url, string apikey) {
            dynamic experts = await ReaderHelper.GetItem($"{url}persons/{expertsId}/press-media?order=date&orderBy=descending&size=100&apiKey={apikey}");
            var press = JsonConvert.DeserializeObject(experts);
            return press.count > 0
                ? ((IEnumerable<dynamic>)press.items)?.Select((pressItem, i) => new ExpertsItem {
                    Title = pressItem.title?.text[0]?.value?.ToString() ?? "",
                    SortOrder = i,
                    Url = pressItem.references[0].url?.ToString() ?? "",
                    Institution = pressItem.references[0].medium?.ToString() ?? "",
                    Year = pressItem.references[0].date?.ToString() ?? "",
                }).ToList() ?? []
                : [];
        }
    }
}
