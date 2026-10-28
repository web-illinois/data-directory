using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace uofi_itp_directory_function {

    public class OpenApiConfigurationOptions : IOpenApiConfigurationOptions {
        public List<IDocumentFilter> DocumentFilters { get; set; } = new List<IDocumentFilter>();

        public bool ForceHttp { get; set; } = false;

        public bool ForceHttps { get; set; } = false;

        public bool IncludeRequestingHostName { get; set; } = true;

        public OpenApiInfo Info { get; set; } = new OpenApiInfo() {
            Version = "1.0.0",
            Title = "WIGG Directory API",
            Description = "<p>List of APIs that allow users to pull information from the WIGG Directory. Use this with the WIGG Directory at <a href='https://directory.wigg.illinois.edu'>https://directory.wigg.illinois.edu</a>.</p> \n\r\n\r",
            Contact = new OpenApiContact() {
                Name = "Bryan Jonker",
                Email = "jonker@illinios.edu",
                Url = new Uri("https://github.com/web-illinois/data-directory"),
            }
        };

        public OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
        public List<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();
    }
}