using Microsoft.AspNetCore.Http;
using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_function.Helpers {

    public static class RequestHelper {
        private const string _jobTypes = "jobTypes";
        private const string _offices = "offices";
        private const string _officeTypes = "officeTypes";
        private const string _searchCriteria = "q";
        private const string _skip = "skip";
        private const string _splitString = "[-]";
        private const string _tags = "tags";
        private const string _take = "take";
        private const string _useFullText = "useFullText";

        public static IEnumerable<string> GetJobTypes(this HttpRequest req) => req.GetArray(_jobTypes);

        public static string GetCodeFromHeader(this HttpRequest req) => req.Headers.FirstOrDefault(h => h.Key == "ilw-key").Value.FirstOrDefault() ?? string.Empty;

        public static IEnumerable<string> GetOffices(this HttpRequest req) => req.GetArray(_offices);

        public static IEnumerable<OfficeTypeEnum> GetOfficesTypes(this HttpRequest req) => req.GetArray(_officeTypes).Where(o => Enum.IsDefined(typeof(OfficeTypeEnum), o)).Select(Enum.Parse<OfficeTypeEnum>);

        public static string GetSearch(this HttpRequest req) => req.Query[_searchCriteria].FirstOrDefault() ?? "";

        public static int GetSkip(this HttpRequest req) => req.Query.ContainsKey(_skip) ? int.Parse(req.Query[_skip].FirstOrDefault() ?? "0") : 0;

        public static IEnumerable<string> GetTags(this HttpRequest req) => req.GetArray(_tags);

        public static int GetTake(this HttpRequest req) => req.Query.ContainsKey(_take) ? int.Parse(req.Query[_take].FirstOrDefault() ?? "10") : 10;

        public static bool GetUseFullText(this HttpRequest req) => bool.Parse(req.Query[_useFullText].FirstOrDefault() ?? "true");

        private static IEnumerable<string> GetArray(this HttpRequest req, string queryname) => (req.Query[queryname].FirstOrDefault() ?? "")?.Split(_splitString).Where(s => !string.IsNullOrWhiteSpace(s)) ?? [];
    }
}