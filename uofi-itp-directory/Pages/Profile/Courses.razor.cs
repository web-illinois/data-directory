using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Helpers;
using uofi_itp_directory_external.ProgramCourse;

namespace uofi_itp_directory.Pages.Profile {
    public partial class Courses {
        private bool _isDirty = false;

        public string AreaCode { get; set; } = "";
        public Employee? Employee { get; set; } = default!;

        [CascadingParameter]
        public LayoutProfile Layout { get; set; } = default!;

        [Parameter]
        public string Refresh { get; set; } = "";

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected DirectoryHookHelper DirectoryHookHelper { get; set; } = default!;

        [Inject]
        protected EmployeeAreaHelper EmployeeAreaHelper { get; set; } = default!;

        [Inject]
        protected EmployeeCourseHelper EmployeeCourseHelper { get; set; } = default!;

        [Inject]
        protected ProgramCourseInformation ProgramCourseInformation { get; set; } = default!;

        [Inject]
        protected EmployeeHelper EmployeeSecurityHelper { get; set; } = default!;
        public string Instructions { get; set; } = "";

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        public async Task Delete(EmployeeCourse course) {
            if (await JsRuntime.InvokeAsync<bool>("confirm", $"This will delete the course \"{course.Title}\". Are you sure?")) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Course deleted");
                _ = await EmployeeCourseHelper.DeleteCourse(course, Employee?.Id ?? 0, Employee?.NetId ?? "", await AuthenticationStateProvider.GetUser());
                _isDirty = false;
            }
        }

        public async Task Load(EmployeeCourse course) {
            if (await JsRuntime.InvokeAsync<bool>("confirm", $"This will delete the course \"{course.Title}\" and replace it with default information. Are you sure?")) {
                var defaultCourse = (await ProgramCourseInformation.GetCourse(AreaCode, course.Rubric, course.CourseNumber));
                if (defaultCourse != null && Employee != null) {
                    course.Title = defaultCourse.Name;
                    course.Url = defaultCourse.Url;
                    course.Description = defaultCourse.Description;
                    course.InternalOrder = 3;
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"Course \"{course.Title}\" refreshed");
                    _isDirty = true;
                } else {
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "No matching course found -- course refresh aborted");
                }
            }
        }

        public async Task LoadAll() {
            if (Employee?.EmployeeCourses.Count != 0) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Will not load courses over existing courses");
            } else {
                if (await JsRuntime.InvokeAsync<bool>("confirm", $"This will load default information. Are you sure?")) {
                    var courses = (await ProgramCourseInformation.GetCourses(AreaCode, Employee?.NetId ?? "")).ToList();
                    if (courses.Any() && Employee != null) {
                        Employee.EmployeeCourses = [.. courses.Select(c => new EmployeeCourse { Title = c.Name, CourseNumber = c.CourseNumber, Description = c.Description, Rubric = c.Rubric, Url = c.Url, InternalOrder = 3 })];
                        Reorder();
                        foreach (var course in Employee.EmployeeCourses) {
                            _ = await EmployeeCourseHelper.SaveCourse(course, Employee?.Id ?? 0, Employee?.NetId ?? "", await AuthenticationStateProvider.GetUser());
                        }
                        _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Courses refreshed");
                    } else {
                        _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "No courses found -- course refresh aborted");
                    }
                    _isDirty = false;
                }
            }
        }

        public void New() {
            if (Employee != null) {
                Employee.EmployeeCourses.Add(new() { InEditState = true });
                _isDirty = true;
            }
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        protected override async Task OnInitializedAsync() {
            Layout.Rebuild();
            var employeeId = CacheHelper.GetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, Refresh);
            Employee = await AccessHelper.GetEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), EmployeeSecurityHelper, employeeId);
            if (Employee == null) {
                throw new Exception("No employee");
            }
            Instructions = await EmployeeAreaHelper.CoursesInstructions(Employee.NetId);
            AreaCode = (await EmployeeSecurityHelper.GetEmployeeSettings(Employee))?.InternalCode ?? "";
            Reorder();
        }

        public async Task RefreshDirectory() {
            if (Employee == null) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "No employee to refresh");
                return;
            }
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"Directory Entry Starting Refresh");
            var results = await DirectoryHookHelper.SendHook(Employee.Id, false);
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", results.isSuccessful ? "Directory Entry refresh complete" : results.results);
        }

        public async Task Save(EmployeeCourse course) {
            if (course.InEditState) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"Course \"{course.Title}\" updated");
                _ = await EmployeeCourseHelper.SaveCourse(course, Employee?.Id ?? 0, Employee?.NetId ?? "", await AuthenticationStateProvider.GetUser());
                course.InEditState = false;
                _isDirty = false;
                Reorder();
            } else {
                course.InEditState = true;
            }
        }

        protected override async Task OnParametersSetAsync() => await OnInitializedAsync();

        protected void SetDirty() => _isDirty = true;

        private async Task LocationChangingHandler(LocationChangingContext arg) {
            if (_isDirty) {
                if (!(await JsRuntime.InvokeAsync<bool>("confirm", $"You have unsaved changes. Are you sure?"))) {
                    arg.PreventNavigation();
                }
            }
        }

        private void Reorder() {
            if (Employee != null && Employee.EmployeeCourses != null) {
                Employee.EmployeeCourses = [.. Employee.EmployeeCourses.OrderBy(a => a.InternalOrder).ThenBy(a => a.Rubric).ThenBy(a => a.CourseNumber).ThenBy(a => a.Title)];
            }
        }
    }
}

