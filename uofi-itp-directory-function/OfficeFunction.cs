using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.Orgchart;
using uofi_itp_directory_function.Helpers;
using uofi_itp_directory_function.ViewModels;

namespace uofi_itp_directory_function {

    public class OfficeFunction {
        private readonly DirectoryRepository _directoryRepository;
        private readonly ILogger<OfficeFunction> _logger;
        private readonly OrgchartHelper _orgchartHelper;
        public OfficeFunction(ILogger<OfficeFunction> logger, DirectoryRepository directoryRepository, OrgchartHelper orgchartHelper) {
            _logger = logger;
            _directoryRepository = directoryRepository;
            _orgchartHelper = orgchartHelper;
        }

        [Function("Office")]
        [OpenApiOperation(operationId: "Office", tags: "Office", Description = "Get an office by ID. This includes office hours and office settings.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The ID of the office you want")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(OfficeInformation), Description = "An office")]
        public async Task<IActionResult> GetOffice([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Office/{id}")] HttpRequest req, int id)
            => new OkObjectResult(await _directoryRepository.ReadAsync(c => c.Offices
                .Include(o => o.OfficeHours).Include(o => o.OfficeSettings).Include(o => o.Area)
                .Where(o => o.IsActive && o.Id == id)
                .Select(o => new OfficeInformation(o)).FirstOrDefault()));

        [Function("OfficeCode")]
        [OpenApiOperation(operationId: "OfficeCode", tags: "Office", Description = "Get an office by a code. This includes office hours and office settings.")]
        [OpenApiParameter(name: "code", In = ParameterLocation.Path, Required = false, Type = typeof(string), Description = "The office code of the office you want")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(OfficeInformation), Description = "An office")]
        public async Task<IActionResult> GetOfficeCode([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Office/Code/{code}")] HttpRequest req, string code)
            => new OkObjectResult(await _directoryRepository.ReadAsync(c => c.Offices
                .Include(o => o.OfficeHours).Include(o => o.OfficeSettings).Include(o => o.Area)
                .Where(o => o.IsActive && o.OfficeSettings.InternalCode == code)
                .Select(o => new OfficeInformation(o)).ToList()));

        [Function("OfficeOrgChart")]
        [OpenApiOperation(operationId: "OfficeOrgChart", tags: "Office", Description = "Get a JSON version of the organization chart used for the ilw-org-chart web component. Note that all items in the org chart need to be filled out for this to work properly.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The ID of the office you want")]
        [OpenApiParameter(name: "includeAllEmployees", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Description = "Whether to include all employees in the org chart")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(OrgchartItem), Description = "JSON of the org chart used for the ilw-org-chart web component.")]
        public async Task<IActionResult> GetOfficeOrgChart([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Office/OrgChart/{id}")] HttpRequest req, int id)
            => new OkObjectResult(await _orgchartHelper.GetOrgchartForOffice(id, req.GetIncludeAllEmployees()));
    }
}