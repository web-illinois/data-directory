using Newtonsoft.Json;

namespace uofi_itp_directory_search.ViewModel {

    public class DirectoryOfficeItem {

        [JsonProperty("address")]
        public string Address { get; set; } = "";

        [JsonProperty("building")]
        public string Building { get; set; } = "";

        [JsonProperty("city")]
        public string City { get; set; } = "";

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("email")]
        public string Email { get; set; } = "";

        [JsonProperty("externalurl")]
        public string ExternalUrl { get; set; } = "";

        [JsonProperty("hourstext")]
        public string HoursText { get; set; } = "";

        [JsonProperty("internalorder")]
        public int InternalOrder { get; set; }

        [JsonProperty("internalurl")]
        public string InternalUrl { get; set; } = "";

        [JsonProperty("map")]
        public string Map { get; set; } = "";

        [JsonProperty("officetype")]
        public string OfficeType { get; set; } = "";

        [JsonProperty("people")]
        public List<EmployeeCompact> People { get; set; } = [];

        [JsonProperty("phone")]
        public string Phone { get; set; } = "";

        [JsonProperty("room")]
        public string Room { get; set; } = "";

        [JsonProperty("state")]
        public string State { get; set; } = "";

        [JsonProperty("ticketurl")]
        public string TicketUrl { get; set; } = "";

        [JsonProperty("title")]
        public string Title { get; set; } = "";

        [JsonProperty("zip")]
        public string Zip { get; set; } = "";
    }
}