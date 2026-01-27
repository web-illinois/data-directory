using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_function.Helpers;
using uofi_itp_directory_search.LoadHelper;

namespace uofi_itp_directory_function {

    public class LoadFunction(ILogger<LoadFunction> logger, QueueManager queueManager, LoadManager loadManager) {
        private readonly LoadManager _loadManager = loadManager;
        private readonly ILogger<LoadFunction> _logger = logger;
        private readonly QueueManager _queueManager = queueManager;

        [Function("ClearLoadProcess")]
        [OpenApiOperation(operationId: "Clear Load Process", tags: "Load", Description = "Clear the load process to restart it. Used if you need to refresh from scratch.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "A status of what it did.")]
        public async Task<IActionResult> ClearLoadProcess([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Load/Clear")] HttpRequest req) =>
            LogAndReturn(await _queueManager.Clear());

        [Function("LoadPersonAutomatically")]
        [OpenApiOperation(operationId: "Load Person Automatically", tags: "Load", Description = "Load a person using the default parameters and source information. Note that this account has to be listed as using the default load for this to work properly.")]
        [OpenApiParameter(name: "name", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The NetID of the person you want to load.")]
        [OpenApiParameter(name: "source", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Source value.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "A status of what it did.")]
        public async Task<IActionResult> LoadPerson([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Load/Person/{source}/{netid}")] HttpRequest req, string source, string netid) => LogAndReturn(await _loadManager.LoadPerson(netid, source));

        [Function("LoadPersonManually")]
        [OpenApiOperation(operationId: "Load Person", tags: "Load", Description = "Load a person sending the json of the body, manually overriding any other options. Note that you need to register your API key with IT Partners to let us know you want to use this option. If you do this, we will not load anything from EDW -- we will rely on you to give us basic information and profile information.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Header)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The NetID of the person you want to load.")]
        [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Source value.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "A status of what it did.")]
        public Task<IActionResult> LoadPersonManually([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Load/Manual")] HttpRequest req) {
            //TODO Need to implement once someone needs it - must include 'var body = new StreamReader(req.Body).ReadToEnd();'
            try {
                return Task.FromResult<IActionResult>(new OkObjectResult("Not implemented yet"));
            } catch (Exception e) {
                return Task.FromResult<IActionResult>(new BadRequestObjectResult(e.Message));
            }
        }

        [Function("LoadProcess")]
        [OpenApiOperation(operationId: "Load Process", tags: "Load", Description = "Start the load process. If there are names to process, it will process those names. If there are no names to process and it has been 12 hours since the last full load, it will load all the names from areas marked with the 'Auto-load profiles to directory hook' option. Otherwise, it will do nothing.")]
        [OpenApiParameter(name: "take", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The number of names to process.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "A status of what it did. If it loaded names, it will list the net IDs of the names that were loaded.")]
        public async Task<IActionResult> Process([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Load")] HttpRequest req) =>
            LogAndReturn(await _queueManager.Process(req.GetTake()));

        private OkObjectResult LogAndReturn(string s) {
            _logger.Log(LogLevel.Debug, s);
            return new OkObjectResult(s);
        }
    }
}