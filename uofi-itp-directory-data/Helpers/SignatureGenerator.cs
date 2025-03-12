using uofi_itp_directory_data.DataModels;

namespace uofi_itp_directory_data.Helpers {

    public class SignatureGenerator {
        private readonly string _webServicesSignatureLink = "";

        public SignatureGenerator(string? webServicesSignatureLink) {
            _webServicesSignatureLink = webServicesSignatureLink ?? "";
        }

        public string GenerateSignatureUrl(Employee employee, string areaInformation) => $"{_webServicesSignatureLink}?{Set("name", employee.GenerateSignatureName(), true)}" +
    $"{Set("department1", employee.PrimaryJobProfile.Office.Title)}" +
    $"{Set("department2", "")}" +
    $"{Set("role1", employee.PrimaryJobProfile.Title)}" +
    $"{Set("role2", "")}" +
    $"{Set("address1", employee.Room + " " + employee.Building)}" +
    $"{Set("address2", employee.AddressLine1)}" +
    $"{Set("cityStateZip", string.IsNullOrWhiteSpace(employee.City) ? "" : employee.City + ", " + employee.State + " " + employee.ZipCode)}" +
    $"{Set("phone", employee.IsPhoneHidden ? "" : employee.Phone)}" +
    $"{Set("email", employee.NetId)}" +
    $"{Set("website1", employee.PrimaryJobProfile.Office.ExternalUrl)}" +
    $"{Set("campus", employee.PrimaryJobProfile.Office.Area.Title)}{(string.IsNullOrWhiteSpace(areaInformation) ? "" : "&" + areaInformation.Trim('&'))}";

        private string Set(string name, string value, bool isFirst = false) {
            return (isFirst ? "" : "&") + (string.IsNullOrWhiteSpace(value) ? name + "=" : name + "=" + value.Replace("&", "%26"));
        }
    }
}