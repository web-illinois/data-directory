using Microsoft.AspNetCore.Components.Authorization;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.ControlHelper {

    public static class CacheHelper {

        public static bool ClearCache(AuthenticationState? authenticationState, CacheHolder? cacheHolder) => authenticationState == null || cacheHolder == null
                ? false
                : cacheHolder.ClearCache(authenticationState.GetName());

        public static AreaOfficeThinObject? GetCachedArea(AuthenticationState? authenticationState, CacheHolder? cacheHolder) => authenticationState == null || cacheHolder == null
                ? null
                : cacheHolder.GetArea(authenticationState.GetName());

        public static int? GetCachedEmployee(AuthenticationState? authenticationState, CacheHolder? cacheHolder, string param) {
            if (authenticationState != null && cacheHolder != null) {
                if (!string.IsNullOrEmpty(param)) {
                    _ = cacheHolder.ClearCache(authenticationState.GetName());
                    return null;
                }
                return cacheHolder.GetEmployee(authenticationState.GetName());
            }
            return null;
        }

        public static AreaOfficeThinObject? GetCachedOffice(AuthenticationState? authenticationState, CacheHolder? cacheHolder) => authenticationState == null || cacheHolder == null
        ? null
        : cacheHolder.GetOffice(authenticationState.GetName());

        public static void SetCachedArea(AuthenticationState? authenticationState, CacheHolder? cacheHolder, AreaOfficeThinObject area) {
            if (authenticationState != null && cacheHolder != null) {
                cacheHolder.SetArea(authenticationState.GetName(), area);
            }
        }

        public static void SetCachedEmployee(AuthenticationState? authenticationState, CacheHolder? cacheHolder, int? employeeId) {
            if (authenticationState != null && cacheHolder != null && employeeId.HasValue) {
                cacheHolder.SetEmployeeId(authenticationState.GetName(), employeeId.Value);
            }
        }

        public static void SetCachedOffice(AuthenticationState? authenticationState, CacheHolder? cacheHolder, AreaOfficeThinObject office) {
            if (authenticationState != null && cacheHolder != null) {
                cacheHolder.SetOffice(authenticationState.GetName(), office);
            }
        }

        private static string GetName(this AuthenticationState authenticationState) => authenticationState.User?.Identity?.Name ?? "";
    }
}