using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class EmployeeJson {
        public EmployeeJson(Employee employee) {
            if (employee == null) {
                return;
            }
            AlternateContact = employee.AlternateContact;
            AlternateContactEmail = employee.AlternateContactEmail;
            AlternateContactPhone = employee.AlternateContactPhone;
            AddressLine1 = employee.AddressLine1;
            AddressLine2 = employee.AddressLine2;
            Biography = employee.Biography;
            Building = employee.Building;
            City = employee.City;
            CVUrl = employee.CVUrl;
            EmployeeHourText = employee.EmployeeHourText;
            IsAddressHidden = employee.IsAddressHidden;
            IsPhoneHidden = employee.IsPhoneHidden;
            LastRefreshed = employee.LastRefreshed;
            ListedNameFirst = employee.ListedNameFirst;
            ListedNameLast = employee.ListedNameLast;
            NetId = employee.NetId;
            Phone = employee.Phone;
            PhotoAltText = employee.PhotoAltText;
            PhotoUrl = employee.PhotoUrl;
            PreferredNameFirst = employee.PreferredNameFirst;
            PreferredNameLast = employee.PreferredNameLast;
            PreferredPronouns = employee.PreferredPronouns;
            PrimaryProfile = employee.PrimaryProfile;
            ProfileUrl = employee.ProfileUrl;
            Room = employee.Room;
            State = employee.State;
            UseAlternateContactAsPrimary = employee.UseAlternateContactAsPrimary;
            UsePrimaryOfficeAsAddress = employee.UsePrimaryOfficeAsAddress;
            ZipCode = employee.ZipCode;
            EmployeeActivities = [.. employee.EmployeeActivities?.Select(ea => new EmployeeActivityJson(ea)) ?? []];
            EmployeeHours = [.. employee.EmployeeHours?.Select(eh => new EmployeeHourJson(eh)) ?? []];
            EmployeeCourses = [.. employee.EmployeeCourses?.Select(ec => new EmployeeCourseJson(ec)) ?? []];
        }
        public string AlternateContact { get; set; } = "";
        public string AlternateContactEmail { get; set; } = "";
        public string AlternateContactPhone { get; set; } = "";
        public string AddressLine1 { get; set; } = "";
        public string AddressLine2 { get; set; } = "";
        public string Biography { get; set; } = "";
        public string Building { get; set; } = "";
        public string City { get; set; } = "";
        public string CVUrl { get; set; } = "";
        public string EmployeeHourText { get; set; } = "";
        public bool IsAddressHidden { get; set; } = false;

        public bool IsPhoneHidden { get; set; } = false;

        public DateTime? LastRefreshed { get; set; }

        public string ListedNameFirst { get; set; } = "";
        public string ListedNameLast { get; set; } = "";

        public string NetId { get; set; } = "";
        public string Phone { get; set; } = "";
        public string PhotoAltText { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public string PreferredNameFirst { get; set; } = "";
        public string PreferredNameLast { get; set; } = "";
        public string PreferredPronouns { get; set; } = "";

        public int? PrimaryProfile { get; set; }
        public string ProfileUrl { get; set; } = "";
        public string Room { get; set; } = "";
        public string State { get; set; } = "";
        public bool UseAlternateContactAsPrimary { get; set; } = false;
        public bool UsePrimaryOfficeAsAddress { get; set; } = false;
        public string ZipCode { get; set; } = "";

        public List<EmployeeActivityJson> EmployeeActivities { get; set; } = [];

        public List<EmployeeHourJson> EmployeeHours { get; set; } = [];

        public List<EmployeeCourseJson> EmployeeCourses { get; set; } = [];

        public Employee ToEmployee() {
            return new Employee {
                AlternateContact = AlternateContact,
                AlternateContactEmail = AlternateContactEmail,
                AlternateContactPhone = AlternateContactPhone,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                Biography = Biography,
                Building = Building,
                City = City,
                CVUrl = CVUrl,
                EmployeeHourText = EmployeeHourText,
                IsAddressHidden = IsAddressHidden,
                IsPhoneHidden = IsPhoneHidden,
                LastRefreshed = LastRefreshed,
                ListedNameFirst = ListedNameFirst,
                ListedNameLast = ListedNameLast,
                NetId = NetId,
                Phone = Phone,
                PhotoAltText = PhotoAltText,
                PhotoUrl = PhotoUrl,
                PreferredNameFirst = PreferredNameFirst,
                PreferredNameLast = PreferredNameLast,
                PreferredPronouns = PreferredPronouns,
                PrimaryProfile = PrimaryProfile,
                ProfileUrl = ProfileUrl,
                Room = Room,
                State = State,
                UseAlternateContactAsPrimary = UseAlternateContactAsPrimary,
                UsePrimaryOfficeAsAddress = UsePrimaryOfficeAsAddress,
                ZipCode = ZipCode,
                EmployeeActivities = [.. EmployeeActivities?.Select(ea => ea.ToEmployeeActivity()) ?? []],
                EmployeeHours = [.. EmployeeHours?.Select(eh => eh.ToEmployeeHour()) ?? []],
                EmployeeCourses = [.. EmployeeCourses?.Select(ec => ec.ToEmployeeCourse()) ?? []]
            };
        }

    }
}
