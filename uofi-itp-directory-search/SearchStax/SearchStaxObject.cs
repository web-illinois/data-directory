using System.Text;
using System.Text.Json;
using uofi_itp_directory_search.ViewModel;

namespace uofi_itp_directory_search.SearchStax {
    public class SearchStaxObject {
        private readonly JsonSerializerOptions _serializer = new() { PropertyNamingPolicy = new JsonNamingPolicyLowerCase() };
        public bool IsValid { get; set; } = true;
        public string Content { get; set; } = "";
        public string Title { get; set; } = "";
        public string Title_txt_en { get; set; } = "";
        public string Category_news_s { get; set; } = "";
        public string Description { get; set; } = "";
        public string Url { get; set; } = "";
        public string Url_t { get; set; } = "";
        public string Id { get; set; } = "";

        public static async Task<SearchStaxObject> Generate(Employee employee) {
            var content = new StringBuilder();
            content.Append($"{employee.FullName}");
            content.Append($" {employee.PrimaryTitle}");
            content.Append($" {employee.PrimaryOffice}");
            content.Append($" {employee.RoomNumber} {employee.Building}");
            content.Append($" {employee.AddressLine1} {employee.AddressLine2}");
            content.Append($" {employee.Biography}");
            content.Append($" {employee.ResearchStatement}");
            content.Append($" {employee.TeachingStatement}");
            var description = $"{employee.FullName} ({employee.Email}): {employee.PrimaryOffice}, {employee.PrimaryTitle}";
            var title = $"{employee.FullName} | Illinois";
            var profileUrl = $"{employee.ProfileUrl}" ?? "";
            return new SearchStaxObject {
                IsValid = true,
                Title = title,
                Title_txt_en = title,
                Description = description,
                Url = profileUrl,
                Url_t = profileUrl,
                Id = profileUrl,
                Category_news_s = "People",
                Content = content.ToString()
            };
        }
        public override string ToString() {
            return "{ \"add\": { \"doc\": " + JsonSerializer.Serialize(this, _serializer) + " } }";
        }
    }
}
