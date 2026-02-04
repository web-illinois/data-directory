using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Helpers;
using uofi_itp_directory_function.Helpers;
using uofi_itp_directory_search.LoadHelper;
using uofi_itp_directory_search.ViewModel;

namespace uofi_itp_directory_function {

    public class LoadFunction(ILogger<LoadFunction> logger, QueueManager queueManager, LoadManager loadManager, ApiHelper apiHelper, PersonSetter personSetter) {
        private readonly ApiHelper _apiHelper = apiHelper;
        private readonly LoadManager _loadManager = loadManager;
        private readonly PersonSetter _personSetter = personSetter;
        private readonly ILogger<LoadFunction> _logger = logger;
        private readonly QueueManager _queueManager = queueManager;

        [Function("LoadPersonAutomatically")]
        [OpenApiOperation(operationId: "Load Person Automatically", tags: "Load", Description = "Load a person using the default parameters and source information. Note that this account has to be listed as using the default load for this to work properly.")]
        [OpenApiParameter(name: "name", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The NetID of the person you want to load.")]
        [OpenApiParameter(name: "source", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "Source value.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "A status of what it did.")]
        public async Task<IActionResult> LoadPerson([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Load/Person/{source}/{netid}")] HttpRequest req, string source, string netid) => LogAndReturn(await _loadManager.LoadPerson(netid, source));

        [Function("LoadPersonManually")]
        [OpenApiOperation(operationId: "Load Person", tags: "Load", Description = "Load a person sending the json of the body, manually overriding any other options. Note that you need to register your API key with the application before you use this option. If you do this, we will not load anything from EDW -- we will rely on you to give us basic information and profile information.")]
        [OpenApiParameter(name: "ilw-key", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "The API Key.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "A status of what it did.")]
        public async Task<IActionResult> LoadPersonManually([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Load/Manual")] HttpRequest req) {
            try {

                var key = req.GetCodeFromHeader();
                var people = await req.ReadFromJsonAsync<IEnumerable<Employee>>() ?? [];
                var sourceName = people.FirstOrDefault()?.Source ?? "";
                var allowApi = await _apiHelper.CheckApi(sourceName, key);
                if (!allowApi) {
                    throw new Exception("API Key in header ilw-key is needed, was sent " + key);
                }
                var count = new List<string>();
                foreach (var person in people) {
                    if (sourceName == person.Source) {
                        if (await _personSetter.SaveSingle(person)) {
                            count.Add(person.NetId);
                        }
                    }
                }
                return new OkObjectResult($"Loaded {count.Count} people: {string.Join(", ", count)}");
            } catch (Exception e) {
                return new BadRequestObjectResult(e.Message);
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