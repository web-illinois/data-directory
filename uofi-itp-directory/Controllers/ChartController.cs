using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using uofi_itp_directory_data.DataAccess;

namespace uofi_itp_directory.Controllers {

    [Route("[controller]")]
    [AllowAnonymous]
    public class ChartController(OfficeHelper? officeHelper) : Controller {
        private readonly OfficeHelper _officeHelper = officeHelper ?? throw new ArgumentNullException("officeHelper ");

        [DisableCors]
        [Route("{officeid}")]
        [HttpGet]
        public async Task<IActionResult> GetJson(int officeid) {
            var jsonString = await _officeHelper.GetOfficeChartJson(officeid);
            return Content(jsonString);
        }

        [DisableCors]
        [Route("{officeid}/file")]
        [HttpGet]
        public async Task<IActionResult> GetFlatFile(int officeid) {
            var file = await _officeHelper.GetOfficeChartFlatFile(officeid);
            return Content(file);
        }

    }
}