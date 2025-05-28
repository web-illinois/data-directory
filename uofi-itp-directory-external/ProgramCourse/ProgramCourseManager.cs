using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace uofi_itp_directory_external.ProgramCourse {

    public class ProgramCourseInformation(string? programCourseUrl) {
        private readonly string _baseUrl = programCourseUrl ?? "";

        private readonly Dictionary<string, string> _courseTranslation = new() {
            { "education", "coe" }
        };

        public virtual IEnumerable<Course> GetCourses(string source, string netid, string uin) {
            var json = "";
            try {
                var url = $"{_baseUrl}?source={GetCollegeType(source)}&netid={netid}&uin={uin}";
                using var client = new HttpClient();
                using var res = client.GetAsync(url).Result;
                using var content = res.Content;
                json = content.ReadAsStringAsync().Result;
                dynamic? data = JsonConvert.DeserializeObject<dynamic>(json);
                JArray? items = data == null ? null : data;
                return items == null || items.Count == 0
                    ? new List<Course>()
                    : (IEnumerable<Course>) items.Select(x => (dynamic) x).Select(item => new Course {
                        CourseNumber = item.CourseNumber,
                        Rubric = item.Rubric,
                        Description = item.Description,
                        Url = item.Url,
                        Name = item.Title
                    }).ToList();
            } catch {
                return new List<Course>();
            }
        }

        private string GetCollegeType(string source) => _courseTranslation.ContainsKey(source) ? _courseTranslation[source] : source;
    }
}