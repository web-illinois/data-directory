using Newtonsoft.Json;

namespace uofi_itp_directory_search.ViewModel {
    public class NewsArticle : BaseItem {
        [JsonProperty("date")]
        public string Date { get; set; } = "";

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("imagealt")]
        public string ImageAlt { get; set; } = "";

        [JsonProperty("imagesrc")]
        public string ImageSrc { get; set; } = "";

        [JsonProperty("source")]
        public string Source { get; set; } = "";
    }
}
