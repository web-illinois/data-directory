using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory_data.DataAccess {
    public class EmployeeCourseHelper(DirectoryRepository directoryRepository, DirectoryHookHelper directoryHookHelper, LogHelper logHelper) {
        private readonly DirectoryHookHelper _directoryHookHelper = directoryHookHelper;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;
        private readonly LogHelper _logHelper = logHelper;

        public async Task<int> DeleteCourse(EmployeeCourse course, int employeeId, string employeeNetId, string changedByNetId) {
            var returnValue = await _directoryRepository.DeleteAsync(course);
            _ = await _logHelper.CreateEmployeeLog(changedByNetId, "Deleted course " + course.Title, course.ToString(), employeeId, employeeNetId);
            _ = _directoryHookHelper.SendHook(employeeId, true);
            return returnValue;
        }

        public async Task<int> SaveCourse(EmployeeCourse course, int employeeId, string employeeNetId, string changedByNetId) {
            course.EmployeeId = employeeId;
            var returnValue = await _directoryRepository.UpdateAsync(course);
            _ = await _logHelper.CreateEmployeeLog(changedByNetId, "Added/changed course " + course.Title, course.ToString(), employeeId, employeeNetId);
            _ = _directoryHookHelper.SendHook(employeeId, true);
            return returnValue;
        }
    }
}
