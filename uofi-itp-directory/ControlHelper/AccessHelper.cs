using Microsoft.AspNetCore.Components.Authorization;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.ControlHelper {

    public static class AccessHelper {

        public static async Task<List<AreaOfficeThinObject>> GetAreas(AuthenticationState? authenticationState, PersonOptionHelper? personOptionHelper) => authenticationState == null || personOptionHelper == null
                ? []
                : [.. await personOptionHelper.Areas(authenticationState.User?.Identity?.Name)];

        public static async Task<Employee?> GetEmployee(AuthenticationState? authenticationState, EmployeeHelper? employeeSecurityHelper, int? id) =>
            authenticationState == null || employeeSecurityHelper == null ? null : (await employeeSecurityHelper.GetEmployee(id, authenticationState.User?.Identity?.Name ?? ""));

        public static async Task<List<AreaOfficeThinObject>> GetOffices(AuthenticationState? authenticationState, PersonOptionHelper? personOptionHelper) => authenticationState == null || personOptionHelper == null
                ? []
                : [.. await personOptionHelper.Offices(authenticationState.User?.Identity?.Name)];

        public static bool IsSingle(this IEnumerable<AreaOfficeThinObject> areaOfficeThinObjects) => areaOfficeThinObjects.Count() == 1;
    }
}