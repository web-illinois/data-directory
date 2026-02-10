using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class OfficeJson {
        public OfficeJson(Office office) {
            if (office == null) {
                return;
            }
            Address = office.Address;
            Audience = office.Audience;
            Building = office.Building;
            BuildingCode = office.BuildingCode;
            CanAddPeople = office.CanAddPeople;
            City = office.City;
            Description = office.Description;
            Email = office.Email;
            ExternalUrl = office.ExternalUrl;
            HoursIncludeHolidayMessage = office.HoursIncludeHolidayMessage;
            HoursTextOverride = office.HoursTextOverride;
            InternalOrder = office.InternalOrder;
            InternalUrl = office.InternalUrl;
            IsInternalOnly = office.IsInternalOnly;
            Notes = office.Notes;
            OfficeHourText = office.OfficeHourText;
            OfficeType = (int)office.OfficeType;
            Phone = office.Phone;
            Room = office.Room;
            State = office.State;
            TicketUrl = office.TicketUrl;
            Title = office.Title;
            ZipCode = office.ZipCode;
            InternalCode = office.OfficeSettings?.InternalCode ?? "";
            InternalNotes = office.OfficeSettings?.InternalNotes ?? "";
            UseJobSpecificDescription = office.OfficeSettings?.UseJobSpecificDescription ?? false;
            OfficeHours = [.. office.OfficeHours?.Select(oh => new OfficeHourJson(oh)) ?? []];
            JobProfiles = [.. office.JobProfiles?.Select(jp => new JobProfileJson(jp)) ?? []];
        }

        public string Address { get; set; } = "";

        public string Audience { get; set; } = "";

        public string Building { get; set; } = "";

        public string BuildingCode { get; set; } = "";

        public bool CanAddPeople { get; set; }
        public string City { get; set; } = "";

        public string Description { get; set; } = "";
        public string Email { get; set; } = "";

        public string ExternalUrl { get; set; } = "";
        public bool HoursIncludeHolidayMessage { get; set; }
        public string HoursTextOverride { get; set; } = "";

        public int InternalOrder { get; set; }

        public string InternalUrl { get; set; } = "";

        public bool IsInternalOnly { get; set; }

        public string Notes { get; set; } = "";
        public string OfficeHourText { get; set; } = "";
        public int OfficeType { get; set; }
        public string Phone { get; set; } = "";
        public string Room { get; set; } = "";
        public string State { get; set; } = "";
        public string TicketUrl { get; set; } = "";
        public string Title { get; set; } = "";
        public string ZipCode { get; set; } = "";

        public string InternalCode { get; set; } = "";
        public string InternalNotes { get; set; } = "";
        public bool UseJobSpecificDescription { get; set; }

        public List<OfficeHourJson> OfficeHours { get; set; } = [];

        public List<JobProfileJson> JobProfiles { get; set; } = [];

        public Office ToOffice() {
            return new Office {
                Address = Address,
                Audience = Audience,
                Building = Building,
                BuildingCode = BuildingCode,
                CanAddPeople = CanAddPeople,
                City = City,
                Description = Description,
                Email = Email,
                ExternalUrl = ExternalUrl,
                HoursIncludeHolidayMessage = HoursIncludeHolidayMessage,
                HoursTextOverride = HoursTextOverride,
                InternalOrder = InternalOrder,
                InternalUrl = InternalUrl,
                IsInternalOnly = IsInternalOnly,
                Notes = Notes,
                OfficeHourText = OfficeHourText,
                OfficeType = (OfficeTypeEnum)OfficeType,
                Phone = Phone,
                Room = Room,
                State = State,
                TicketUrl = TicketUrl,
                Title = Title,
                ZipCode = ZipCode,
                OfficeSettings = new OfficeSettings {
                    InternalCode = InternalCode,
                    InternalNotes = InternalNotes,
                    UseJobSpecificDescription = UseJobSpecificDescription
                },
                OfficeHours = [.. OfficeHours?.Select(oh => oh.ToOfficeHour()) ?? []],
                JobProfiles = [.. JobProfiles?.Select(jp => jp.ToJobProfile()) ?? []]
            };
        }
    }
}
