using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace uofi_itp_directory_external.ProgramCourse {

    public class ProgramCourseInformation(string? programCourseUrl) {
        private readonly string _baseUrl = programCourseUrl?.Trim('/') ?? "https://programcourseapi.itpartners.illinois.edu/api/CourseByFaculty";

        private readonly Dictionary<string, string> _courseTranslation = new() {
            { "education", "coe" }
        };

        public virtual async Task<IEnumerable<Course>> GetCourses(string source, string netid) {
            if (string.IsNullOrWhiteSpace(source)) {
                return [];
            }
            try {
                var url = $"{GetCourseFacultyUrl()}?source={GetCollegeType(source)}&netid={netid.Replace("@illinois.edu", "")}";
                using var client = new HttpClient();
                using var res = await client.GetAsync(url);
                using var content = res.Content;
                var json = await content.ReadAsStringAsync();
                dynamic? data = JsonConvert.DeserializeObject<dynamic>(json);
                JArray? items = data ?? null;
                return items == null || items.Count == 0
                    ? []
                    : items.Select(x => (dynamic)x).Select(item => new Course {
                        CourseNumber = item.CourseNumber,
                        Rubric = item.Rubric,
                        Description = item.Description,
                        Url = item.Url,
                        Name = item.CourseTitle
                    }).ToList();
            } catch {
                return [];
            }
        }

        public virtual async Task<Course> GetCourse(string source, string rubric, string courseNumber) {
            if (!string.IsNullOrWhiteSpace(source)) {
                try {
                    var url = $"{GetCourseUrl()}?id={GetCollegeType(source)}-{rubric.ToUpperInvariant()}-{courseNumber}";
                    using var client = new HttpClient();
                    using var res = await client.GetAsync(url);
                    using var content = res.Content;
                    var json = await content.ReadAsStringAsync();
                    dynamic? data = JsonConvert.DeserializeObject<dynamic>(json);
                    if (data != null && data?.Rubric != "" && data?.CourseNumber != "") {
                        return new Course {
                            CourseNumber = data?.CourseNumber ?? "",
                            Rubric = data?.Rubric ?? "",
                            Description = data?.Description ?? "",
                            Url = data?.Url ?? "",
                            Name = data?.CourseTitle ?? ""
                        };
                    }
                } catch {
                    // Do nothing, go to next step
                }
            }
            try {
                var url = $"{GetCampusCourseUrl()}?rubric={rubric.ToUpperInvariant()}&coursenumber={courseNumber}&singlesemester=true&courseonly=true";
                using var client = new HttpClient();
                using var res = await client.GetAsync(url);
                using var content = res.Content;
                var json = await content.ReadAsStringAsync();
                dynamic? data = JsonConvert.DeserializeObject<dynamic>(json);
                return data == null
                    ? new Course()
                    : new Course {
                        CourseNumber = data.CourseNumber,
                        Rubric = data.Rubric,
                        Description = data.Description,
                        Url = data.Url,
                        Name = data.CourseTitle
                    };
            } catch {
                return new Course();
            }
        }

        private string GetCollegeType(string source) => _courseTranslation.ContainsKey(source) ? _courseTranslation[source] : source;
        private string GetCourseUrl() => _baseUrl.EndsWith("CourseByFaculty") ? _baseUrl.Replace("CourseByFaculty", "Course") : _baseUrl + "/Course";
        private string GetCampusCourseUrl() => _baseUrl.EndsWith("CourseByFaculty") ? _baseUrl.Replace("CourseByFaculty", "CampusCourse") : _baseUrl + "/CampusCourse";
        private string GetCourseFacultyUrl() => _baseUrl.EndsWith("CourseByFaculty") ? _baseUrl : _baseUrl + "/CourseByFaculty";
    }
}