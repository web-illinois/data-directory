using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using uofi_itp_directory_external.DirectoryAccess;

namespace uofi_itp_directory.Controllers {

    [Route("[controller]")]
    [AllowAnonymous]
    public class CampusExportController() : Controller {

        [Route("{source}/{netid}")]
        [HttpGet]
        public async Task<IActionResult> ByUsername(string source, string netid) {
            var jsonString = await DirectoryReader.GetPersonJson(netid, source);
            if (string.IsNullOrWhiteSpace(jsonString)) {
                return NotFound();
            }
            dynamic? person = JsonConvert.DeserializeObject(jsonString);
            var show = false;
            if (person != null || person?.username?.ToString() != "" && person?.jobProfiles != null) {
                foreach (var profile in person?.jobProfiles) {
                    if (profile.displayOrder > 0) {
                        show = true;
                    }
                }
            }
            return person == null || person?.username?.ToString() == "" || !show ? NotFound() : View("CampusExportV1", person);
        }
    }
}