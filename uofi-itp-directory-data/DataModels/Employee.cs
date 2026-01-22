using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace uofi_itp_directory_data.DataModels {

    public class Employee : BaseDataItem {
        public string AlternateContact { get; set; } = "";
        public string AlternateContactEmail { get; set; } = "";
        public string AlternateContactPhone { get; set; } = "";
        public string AddressLine1 { get; set; } = "";
        public string AddressLine2 { get; set; } = "";
        public string Biography { get; set; } = "";
        public string Building { get; set; } = "";
        public string City { get; set; } = "";
        public string CVUrl { get; set; } = "";
        public virtual ICollection<EmployeeActivity> EmployeeActivities { get; set; } = default!;
        public virtual ICollection<EmployeeCourse> EmployeeCourses { get; set; } = default!;
        public virtual ICollection<EmployeeHour> EmployeeHours { get; set; } = default!;
        [NotMapped]
        public string Email { get; set; } = "";
        public string EmployeeHourText { get; set; } = "";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public bool IsAddressHidden { get; set; } = false;

        [NotMapped]
        public bool IsCurrentUser { get; set; } = false;

        [NotMapped]
        public bool IsEntryDisabled { get; set; } = true;

        public bool IsPhoneHidden { get; set; } = false;

        // used by the code to determine if the user can edit this object
        public virtual ICollection<JobProfile> JobProfiles { get; set; } = default!;

        public DateTime? LastRefreshed { get; set; }

        public string ListedNameFirst { get; set; } = "";
        public string ListedNameLast { get; set; } = "";

        [NotMapped]
        public string Name => string.IsNullOrWhiteSpace(NameFirst) || string.IsNullOrWhiteSpace(NameLast) ? "" : $"{NameFirst} {NameLast}";

        [NotMapped]
        public string NameFirst => string.IsNullOrWhiteSpace(PreferredNameFirst) ? ListedNameFirst : PreferredNameFirst;

        [NotMapped]
        public string NameLast => string.IsNullOrWhiteSpace(PreferredNameLast) ? ListedNameLast : PreferredNameLast;

        [NotMapped]
        public string NameLinked => Regex.Replace(Name, "[^A-Za-z0-9 ]", "").Replace(" ", "-").ToLowerInvariant();

        [NotMapped]
        public string NameReversed => string.IsNullOrWhiteSpace(NameFirst) || string.IsNullOrWhiteSpace(NameLast) ? "" : $"{NameLast}, {NameFirst}";

        // TODO Is this really a NetID or an email? Need to nail this down
        public string NetId { get; set; } = "";

        [NotMapped]
        public string NetIdTruncated => NetId?.ToLowerInvariant().Replace("@illinois.edu", "").ToLowerInvariant() ?? "";

        public string Phone { get; set; } = "";
        public string PhotoAltText { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public string PreferredNameFirst { get; set; } = "";
        public string PreferredNameLast { get; set; } = "";
        public string PreferredPronouns { get; set; } = "";

        [NotMapped]
        public JobProfile PrimaryJobProfile => JobProfiles.FirstOrDefault(jp => PrimaryProfile == null || jp.OfficeId == PrimaryProfile) ?? JobProfiles.FirstOrDefault() ?? new JobProfile();

        public int? PrimaryProfile { get; set; }

        [NotMapped]
        public string ProfileName => IsCurrentUser ? "My Profile" : Name;

        public string ProfileUrl { get; set; } = "";
        public string Room { get; set; } = "";
        public string State { get; set; } = "";
        public bool UseAlternateContactAsPrimary { get; set; } = false;
        public bool UsePrimaryOfficeAsAddress { get; set; } = false;
        public string ZipCode { get; set; } = "";

        public void PrepareForSearch() {
            if (UseAlternateContactAsPrimary) {
                Email = AlternateContactEmail;
                Phone = AlternateContactPhone;
            } else {
                Email = NetId;
            }
        }

        public string GenerateSignatureName() {
            if (string.IsNullOrWhiteSpace(PreferredNameFirst) && string.IsNullOrWhiteSpace(ListedNameFirst) && string.IsNullOrWhiteSpace(PreferredNameLast) && string.IsNullOrWhiteSpace(ListedNameLast)) {
                return "";
            }
            var name = !string.IsNullOrWhiteSpace(PreferredNameFirst) ? PreferredNameFirst : !string.IsNullOrWhiteSpace(ListedNameFirst) ? ListedNameFirst : "";
            name += " " + (!string.IsNullOrWhiteSpace(PreferredNameLast) ? PreferredNameLast : !string.IsNullOrWhiteSpace(ListedNameLast) ? ListedNameLast : "");
            if (!string.IsNullOrWhiteSpace(PreferredPronouns)) {
                name += $" ({PreferredPronouns})";
            }
            return name.Trim();
        }
    }
}