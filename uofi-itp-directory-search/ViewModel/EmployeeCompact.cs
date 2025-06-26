using Newtonsoft.Json;

namespace uofi_itp_directory_search.ViewModel {

    public class EmployeeCompact {

        [JsonProperty("building")]
        public string Building { get; set; } = "";

        [JsonProperty("city")]
        public string City { get; set; } = "";

        [JsonProperty("cvurl")]
        public string CvUrl { get; set; } = "";

        [JsonProperty("email")]
        public string Email { get; set; } = "";

        [JsonProperty("firstname")]
        public string FirstName { get; set; } = "";

        [JsonProperty("fullname")]
        public string FullName => $"{FirstName} {LastName}";

        [JsonProperty("fullnamereversed")]
        public string FullNameReversed => $"{LastName}, {FirstName}";

        [JsonProperty("hours")]
        public string Hours { get; set; } = "";

        [JsonProperty("imagealttext")]
        public string ImageAltText { get; set; } = "";

        [JsonProperty("imageurl")]
        public string ImageUrl { get; set; } = "";

        [JsonProperty("jobprofiles")]
        public List<JobProfile> JobProfiles { get; set; } = default!;

        [JsonProperty("keywords")]
        public List<string> Keywords { get; set; } = default!;

        [JsonProperty("lastname")]
        public string LastName { get; set; } = "";

        [JsonProperty("linkname")]
        public string LinkName { get; set; } = "";

        [JsonProperty("netid")]
        public string NetId { get; set; } = "";

        [JsonProperty("phone")]
        public string Phone { get; set; } = "";

        [JsonProperty("phoneformatted")]
        public string PhoneFormatted => !Phone.Contains('-') && Phone.Length == 7
            ? "217-" + Phone.Insert(3, "-") : !Phone.Contains('-') && Phone.Length == 10
            ? Phone.Insert(6, "-").Insert(3, "-") : Phone;

        [JsonProperty("preferredpronouns")]
        public string PreferredPronouns { get; set; } = "";

        [JsonProperty("primaryoffice")]
        public string PrimaryOffice { get; set; } = "";

        [JsonProperty("primarytitle")]
        public string PrimaryTitle { get; set; } = "";

        [JsonProperty("profileurl")]
        public string ProfileUrl { get; set; } = "";

        [JsonProperty("roomnumber")]
        public string RoomNumber { get; set; } = "";

        [JsonProperty("source")]
        public string Source { get; set; } = "";

        [JsonProperty("state")]
        public string State { get; set; } = "";

        [JsonProperty("uin")]
        public string Uin { get; set; } = "";

        [JsonProperty("zip")]
        public string Zip { get; set; } = "";

        public static EmployeeCompact TransferPrimaryOfficeAndTitle(EmployeeCompact e, string officeName) {
            return e.JobProfiles.Count == 1
                ? e
                : new EmployeeCompact {
                    JobProfiles = e.JobProfiles.Where(jp => jp.Office == officeName).ToList(),
                    PrimaryOffice = e.JobProfiles.FirstOrDefault(jp => jp.Office == officeName)?.Office ?? e.JobProfiles.First().Office,
                    PrimaryTitle = e.JobProfiles.FirstOrDefault(jp => jp.Office == officeName)?.Title ?? e.JobProfiles.First().Title,
                    Building = e.Building,
                    City = e.City,
                    CvUrl = e.CvUrl,
                    Email = e.Email,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Hours = e.Hours,
                    ImageUrl = e.ImageUrl,
                    Keywords = e.Keywords,
                    LinkName = e.LinkName,
                    NetId = e.NetId,
                    Phone = e.Phone,
                    PreferredPronouns = e.PreferredPronouns,
                    ProfileUrl = e.ProfileUrl,
                    RoomNumber = e.RoomNumber,
                    Source = e.Source,
                    State = e.State,
                    Uin = e.Uin,
                    Zip = e.Zip
                };
        }
    }
}