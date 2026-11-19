using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_external.DataWarehouse;
using uofi_itp_directory_function.ViewModels;

namespace uofi_itp_directory_function {

    public class EdwRefreshFunction(ILogger<EdwRefreshFunction> logger, DataWarehouseManager dataWarehouseManager, DirectoryRepository directoryRepository, EmployeeHelper employeeHelper) {
        private readonly DataWarehouseManager _dataWarehouseManager = dataWarehouseManager;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;
        private readonly EmployeeHelper _employeeHelper = employeeHelper;
        private readonly ILogger<EdwRefreshFunction> _logger = logger;

        [Function("EdwPull")]
        [OpenApiOperation(operationId: "EdwPull", tags: "Load", Description = "Look up first and last names of users from EDW")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(NameInformation), Description = "First and Last Name of person")]
        public async Task<IActionResult> Pull([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "EdwPull/{username}")] HttpRequest req, string username) {
            var edw = await _dataWarehouseManager.GetDataWarehouseItem(username);
            return new OkObjectResult(new NameInformation() { NetId = username, FirstName = edw.FirstName, LastName = edw.LastName });
        }

        [Function("EdwRefresh")]
        [OpenApiOperation(operationId: "EdwRefresh", tags: "Load", Description = "Purge the directory database if names are no longer in EDW. This will run a long time.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "A status of what it did. If it deleted names, it will list the net IDs of the names that were deleted.")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req) {
            var returnValue = new List<string>();
            foreach (var name in await _directoryRepository.ReadAsync(r => r.Employees.Select(e => e.NetId).Distinct().OrderBy(n => n))) {
                if ((await _dataWarehouseManager.GetDataWarehouseItem(name)).NetId == "") {
                    _ = await _employeeHelper.DeleteEmployee(name);
                    returnValue.Add(name);
                }
            }
            _logger.Log(LogLevel.Debug, string.Join("; ", returnValue));
            return new OkObjectResult(string.Join("; ", returnValue));
        }
    }
}