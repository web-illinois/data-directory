using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory_data.DataAccess {

    public class EmployeeActivityHelper(DirectoryRepository directoryRepository, DirectoryHookHelper directoryHookHelper, LogHelper logHelper) {
        private readonly DirectoryHookHelper _directoryHookHelper = directoryHookHelper;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;
        private readonly LogHelper _logHelper = logHelper;

        public async Task<int> DeleteActivity(EmployeeActivity activity, int employeeId, string employeeNetId, string changedByNetId) {
            var returnValue = await _directoryRepository.DeleteAsync(activity);
            _ = await _logHelper.CreateEmployeeLog(changedByNetId, "Deleted activity " + activity.Title, activity.ToString(), employeeId, employeeNetId);
            _ = _directoryHookHelper.SendHook(employeeId, true, false);
            return returnValue;
        }

        public async Task<int> SaveActivity(EmployeeActivity activity, int employeeId, string employeeNetId, string changedByNetId) {
            activity.EmployeeId = employeeId;
            var returnValue = await _directoryRepository.UpdateAsync(activity);
            _ = await _logHelper.CreateEmployeeLog(changedByNetId, "Added/changed activity " + activity.Title, activity.ToString(), employeeId, employeeNetId);
            _ = _directoryHookHelper.SendHook(employeeId, true, false);
            return returnValue;
        }
    }
}