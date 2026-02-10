using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.DataJson {
    public class AreaJson {
        public AreaJson() {
        }

        public AreaJson(Area area) {
            if (area == null) {
                return;
            }
            AreaType = (int)area.AreaType;
            Audience = area.Audience;
            ExternalUrl = area.ExternalUrl;
            InternalOrder = area.InternalOrder;
            InternalUrl = area.InternalUrl;
            IsInternalOnly = area.IsInternalOnly;
            Notes = area.AreaSettings.InternalNotes;
            Title = area.Title;
            AllowAdministratorsAccessToPeople = area.AreaSettings.AllowAdministratorsAccessToPeople;
            AllowBeta = area.AreaSettings.AllowBeta;
            AllowInformationForIllinoisExpertsMembers = area.AreaSettings.AllowInformationForIllinoisExpertsMembers;
            AllowPeople = area.AreaSettings.AllowPeople;
            AutoloadProfiles = area.AreaSettings.AutoloadProfiles;
            InstructionsEmployee = area.AreaSettings.InstructionsEmployee;
            InstructionsEmployeeActivities = area.AreaSettings.InstructionsEmployeeActivities;
            InstructionsEmployeeCourses = area.AreaSettings.InstructionsEmployeeCourses;
            InstructionsEmployeeCv = area.AreaSettings.InstructionsEmployeeCv;
            InstructionsEmployeeHeadshot = area.AreaSettings.InstructionsEmployeeHeadshot;
            InstructionsEmployeeSignature = area.AreaSettings.InstructionsEmployeeSignature;
            InstructionsOffice = area.AreaSettings.InstructionsOffice;
            InternalCode = area.AreaSettings.InternalCode;
            InternalNotes = area.AreaSettings.InternalNotes;
            PictureHeight = area.AreaSettings.PictureHeight;
            PictureWidth = area.AreaSettings.PictureWidth;
            SignatureExtension = area.AreaSettings.SignatureExtension;
            UrlPeopleRefresh = area.AreaSettings.UrlPeopleRefresh;
            UrlPeopleRefreshType = (int)area.AreaSettings.UrlPeopleRefreshType;
            UrlProfile = area.AreaSettings.UrlProfile;
            AreaTags = [.. area.AreaTags?.Select(at => new AreaTagJson(at)) ?? []];
            Offices = [.. area.Offices?.Select(o => new OfficeJson(o)) ?? []];
        }

        public List<AreaTagJson> AreaTags { get; set; } = new();
        public List<OfficeJson> Offices { get; set; } = new();
        public int AreaType { get; set; }

        public string Audience { get; set; } = "";

        public string ExternalUrl { get; set; } = "";

        public int InternalOrder { get; set; }

        public string InternalUrl { get; set; } = "";

        public bool IsInternalOnly { get; set; }
        public string Notes { get; set; } = "";

        public string Title { get; set; } = "";

        public bool AllowAdministratorsAccessToPeople { get; set; } = true;
        public bool AllowBeta { get; set; } = false;

        public bool AllowInformationForIllinoisExpertsMembers { get; set; } = false;
        public bool AllowPeople { get; set; } = false;
        public bool AutoloadProfiles { get; set; } = false;

        public string InstructionsEmployee { get; set; } = "";
        public string InstructionsEmployeeActivities { get; set; } = "";
        public string InstructionsEmployeeCourses { get; set; } = "";
        public string InstructionsEmployeeCv { get; set; } = "";
        public string InstructionsEmployeeHeadshot { get; set; } = "";
        public string InstructionsEmployeeSignature { get; set; } = "";
        public string InstructionsOffice { get; set; } = "";
        public string InternalCode { get; set; } = "";
        public string InternalNotes { get; set; } = "";
        public int PictureHeight { get; set; }
        public int PictureWidth { get; set; }
        public string SignatureExtension { get; set; } = "";
        public string UrlPeopleRefresh { get; set; } = "";

        public int UrlPeopleRefreshType { get; set; }
        public string UrlProfile { get; set; } = "";

        public Area ToArea() {
            return new Area {
                AreaType = (AreaTypeEnum)AreaType,
                Audience = Audience,
                ExternalUrl = ExternalUrl,
                InternalOrder = InternalOrder,
                InternalUrl = InternalUrl,
                IsInternalOnly = IsInternalOnly,
                Title = Title,
                AreaSettings = new AreaSettings {
                    AllowAdministratorsAccessToPeople = AllowAdministratorsAccessToPeople,
                    AllowBeta = AllowBeta,
                    AllowInformationForIllinoisExpertsMembers = AllowInformationForIllinoisExpertsMembers,
                    AllowPeople = AllowPeople,
                    AutoloadProfiles = AutoloadProfiles,
                    InstructionsEmployee = InstructionsEmployee,
                    InstructionsEmployeeActivities = InstructionsEmployeeActivities,
                    InstructionsEmployeeCourses = InstructionsEmployeeCourses,
                    InstructionsEmployeeCv = InstructionsEmployeeCv,
                    InstructionsEmployeeHeadshot = InstructionsEmployeeHeadshot,
                    InstructionsEmployeeSignature = InstructionsEmployeeSignature,
                    InstructionsOffice = InstructionsOffice,
                    InternalCode = InternalCode,
                    InternalNotes = InternalNotes,
                    PictureHeight = PictureHeight,
                    PictureWidth = PictureWidth,
                    SignatureExtension = SignatureExtension,
                    UrlPeopleRefresh = UrlPeopleRefresh,
                    UrlPeopleRefreshType = (PeopleRefreshTypeEnum)UrlPeopleRefreshType,
                    UrlProfile = UrlProfile
                },
                AreaTags = [.. AreaTags?.Select(at => at.ToAreaTag()) ?? []],
                Offices = [.. Offices?.Select(o => o.ToOffice()) ?? []]
            };
        }
    }
}
