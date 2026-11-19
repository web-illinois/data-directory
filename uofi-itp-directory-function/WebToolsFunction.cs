using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.WebTools;

namespace uofi_itp_directory_function;

public class WebToolsFunction(ILogger<WebToolsFunction> logger, WebtoolsManager webtoolsManager, OfficeWebtoolHelper officeWebtoolHelper) {
    private readonly ILogger<WebToolsFunction> _logger = logger;
    private readonly WebtoolsManager _webtoolsManager = webtoolsManager;
    private readonly OfficeWebtoolHelper _officeWebtoolHelper = officeWebtoolHelper;

    [Function("ImportPerson")]
    [OpenApiOperation(operationId: "ImportPerson", tags: "Webtools", Description = "Import people from webtools")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The number of people imported from webtools, -1 if the schema does not match.")]
    public async Task<IActionResult> Person([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Webtools/ImportPerson")] HttpRequest req) =>
        new OkObjectResult(await _webtoolsManager.UpdateToolkitData(ToolkitTypeEnum.Person));

    [Function("ImportService")]
    [OpenApiOperation(operationId: "ImportService", tags: "Webtools", Description = "Import services from webtools")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The number of services imported from webtools, -1 if the schema does not match.")]
    public async Task<IActionResult> Service([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Webtools/ImportService")] HttpRequest req) =>
    new OkObjectResult(await _webtoolsManager.UpdateToolkitData(ToolkitTypeEnum.Service));

    [Function("ImportDepartment")]
    [OpenApiOperation(operationId: "ImportDepartment", tags: "Webtools", Description = "Import departments from webtools")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The number of departments imported from webtools, -1 if the schema does not match.")]
    public async Task<IActionResult> Department([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Webtools/ImportDepartment")] HttpRequest req) =>
    new OkObjectResult(await _webtoolsManager.UpdateToolkitData(ToolkitTypeEnum.Department));

    [Function("UpdateOffice")]
    [OpenApiOperation(operationId: "UpdateOffice", tags: "Webtools", Description = "Update office from webtools")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The number of offices updated from webtools, -1 if the schema does not match.")]
    public async Task<IActionResult> UpdateOffice([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Webtools/UpdateOffice")] HttpRequest req) =>
    new OkObjectResult(await _officeWebtoolHelper.ImportOffice());
}