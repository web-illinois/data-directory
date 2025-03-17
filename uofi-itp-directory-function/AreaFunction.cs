using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Helpers;
using uofi_itp_directory_function.Helpers;
using uofi_itp_directory_function.ViewModels;
using uofi_itp_directory_search.ViewModel;

namespace uofi_itp_directory_function {

    public class AreaFunction {
        private readonly DirectoryRepository _directoryRepository;
        private readonly ILogger<AreaFunction> _logger;

        public AreaFunction(ILogger<AreaFunction> logger, DirectoryRepository directoryRepository) {
            _logger = logger;
            _directoryRepository = directoryRepository;
        }

        [Function("AllAreas")]
        [OpenApiOperation(operationId: "AllAreas", tags: "Areas", Description = "Get all active areas. This includes offices, office hours, office settings, and area settings.")]
        [OpenApiParameter(name: "q", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A search term to limit the areas you get")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<AreaInformation>), Description = "The list of areas")]
        public async Task<IActionResult> AllAreas([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Area/All")] HttpRequest req) {
            var search = req.GetSearch();
            var officeTypes = req.GetOfficesTypes();
            return new OkObjectResult(await _directoryRepository.ReadAsync(c => c.Areas
                .Include(a => a.AreaSettings).Include(a => a.AreaTags)
                .Include(a => a.Offices).ThenInclude(o => o.OfficeHours)
                .Include(a => a.Offices).ThenInclude(o => o.OfficeSettings)
                .Where(a => a.IsActive && (!officeTypes.Any() || a.Offices.Any(o => officeTypes.Contains(o.OfficeType))) && (search == "" || a.Title.Contains(search) || a.Audience.Contains(search) ||
                    a.Offices.Any(o => o.Title.Contains(search)) || a.Offices.Any(o => o.Audience.Contains(search))))
                .OrderBy(a => a.InternalOrder).ThenBy(a => a.Title)
                .Select(a => new AreaInformation(a, false, officeTypes)).ToList().Where(a => a.Offices.Any()).ToList()));
        }

        [Function("AllAreasExternal")]
        [OpenApiOperation(operationId: "AllAreasExternal", tags: "Areas", Description = "Get all active areas marked as external. This includes offices, office hours, office settings, and area settings.")]
        [OpenApiParameter(name: "q", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A search term to limit the areas you get")]
        [OpenApiParameter(name: "officeTypes", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A search term to limit the areas you get")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<AreaInformation>), Description = "The list of areas")]
        public async Task<IActionResult> AllAreasExternal([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Area/External")] HttpRequest req) {
            var search = req.GetSearch();
            var officeTypes = req.GetOfficesTypes();
            return new OkObjectResult(await _directoryRepository.ReadAsync(c => c.Areas
                .Include(a => a.AreaSettings).Include(a => a.AreaTags)
                .Include(a => a.Offices).ThenInclude(o => o.OfficeHours)
                .Include(a => a.Offices).ThenInclude(o => o.OfficeSettings)
                .Where(a => a.IsActive && !a.IsInternalOnly && (!officeTypes.Any() || a.Offices.Any(o => officeTypes.Contains(o.OfficeType))) && (search == "" || a.Title.Contains(search) || a.Audience.Contains(search) || a.Offices.Any(o => o.Title.Contains(search)) || a.Offices.Any(o => o.Audience.Contains(search))))
                .OrderBy(a => a.InternalOrder).ThenBy(a => a.Title)
                .Select(a => new AreaInformation(a, true, officeTypes)).ToList().Where(a => a.Offices.Any()).ToList()));
        }

        [Function("Area")]
        [OpenApiOperation(operationId: "Area", tags: "Areas", Description = "Get a single area by ID. This includes offices, office hours, office settings, and area settings.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = false, Type = typeof(string), Description = "The area ID")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(AreaInformation), Description = "A single area")]
        public async Task<IActionResult> GetArea([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Area/Id/{id}")] HttpRequest req, int id) {
            var officeTypes = req.GetOfficesTypes();
            return new OkObjectResult(await _directoryRepository.ReadAsync(c => c.Areas
                .Include(a => a.AreaSettings)
                .Include(a => a.AreaTags)
                .Include(a => a.Offices).ThenInclude(o => o.OfficeHours)
                .Include(a => a.Offices).ThenInclude(o => o.OfficeSettings)
                .Where(a => a.IsActive && a.Id == id)
                .Select(a => new AreaInformation(a, false, officeTypes)).ToList().Where(a => a.Offices.Any()).FirstOrDefault()));
        }

        [Function("AreaCode")]
        [OpenApiOperation(operationId: "AreaCode", tags: "Areas", Description = "Get a single area by area code. This includes offices, office hours, office settings, and area settings.")]
        [OpenApiParameter(name: "code", In = ParameterLocation.Path, Required = false, Type = typeof(string), Description = "The area code")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(AreaInformation), Description = "A single area")]
        public async Task<IActionResult> GetAreaCode([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Area/Code/{code}")] HttpRequest req, string code) {
            var officeTypes = req.GetOfficesTypes();
            return new OkObjectResult(await _directoryRepository.ReadAsync(c => c.Areas
                .Include(a => a.AreaSettings)
                .Include(a => a.AreaTags)
                .Include(a => a.Offices).ThenInclude(o => o.OfficeHours)
                .Include(a => a.Offices).ThenInclude(o => o.OfficeSettings)
                .Where(a => a.IsActive && a.AreaSettings.InternalCode == code)
                .Select(a => new AreaInformation(a, false, officeTypes)).ToList().Where(a => a.Offices.Any()).FirstOrDefault()));
        }

        [Function("JobTypes")]
        [OpenApiOperation(operationId: "JobTypes", tags: "Areas", Description = "Get all job types. ")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<AreaInformation>), Description = "The list of job types")]
        public async Task<IActionResult> JobTypes([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Area/JobTypes")] HttpRequest req) {
            return new OkObjectResult(Enum.GetValues<ProfileCategoryTypeEnum>().Select(o => new JobTypeInformation { Name = o.ToString().ToLowerInvariant(), PrettyName = o.ToPrettyString() }).OrderBy(o => o.PrettyName));
        }
    }
}