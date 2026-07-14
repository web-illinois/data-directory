# IT Partners Directory - README

## Summary

This solution provides a comprehensive directory system for managing and displaying employee, office, and organizational information. It consists of a Blazor web application and an Azure Functions API backend, replacing the legacy people function app and contact support app.

## Production Locations

The system is deployed to two production environments:

* **API**: https://directoryapi.itpartners.illinois.edu
  - Azure Functions application with database connection
  - Serves area, office, employee, and directory information via REST API

* **Web Application**: https://directory.itpartners.illinois.edu
  - Blazor Server application with database connection
  - Provides user interface for directory management and search

## Architecture Overview

### Loopback Process

The system uses a loopback architecture where:
1. The Blazor application calls the Azure Function API
2. The Azure Function API may call itself through the LoadProcess for certain operations

**Important**: When testing in staging environments, ensure App Settings point to the correct URL:
- Production: `directoryapi.wigg.illinois.edu`
- Staging: `directoryapi-staging.wigg.illinois.edu`

### Project Structure

The solution contains **five projects** targeting .NET 8:

#### 1. **uofi-itp-directory** (Blazor Web Application)
- **Type**: Blazor Server application
- **Purpose**: User-facing web interface for directory management
- **Key Dependencies**:
  - Microsoft.Identity.Web (Authentication via Azure AD)
  - Microsoft.EntityFrameworkCore.Design (Database migrations)
  - Blazored.TextEditor (Rich text editing)
  - SixLabors.ImageSharp (Image processing)
- **Key Features**:
  - Azure AD authentication (OpenID Connect)
  - Profile management (biography, headshot, hours, activities)
  - Office and unit management
  - Search functionality
  - Admin controls
  - User secrets support (UserSecretsId: `aspnet-uofi_itp_directory-8fe4dac8-ad41-45e6-b93b-5b6743fefc45`)

#### 2. **uofi-itp-directory-function** (Azure Functions)
- **Type**: Azure Functions v4 application
- **Purpose**: API backend for data operations and integrations
- **Key Dependencies**:
  - Microsoft.Azure.Functions.Worker (v1.22.0)
  - Microsoft.Azure.Functions.Worker.Extensions.OpenApi (Swagger/OpenAPI support)
  - Microsoft.ApplicationInsights (Telemetry and monitoring)
  - SixLabors.ImageSharp (Image processing)
- **Key Functions**:
  - AreaFunction: Area/department management
  - OfficeFunction: Office information endpoints
  - EmployeeFunction: Employee data operations
  - DirectoryFunction: Directory search and queries
  - LoadFunction: Data loading from external sources
  - EdwRefreshFunction: Enterprise Data Warehouse refresh
- **User Secrets**: UserSecretsId `67f6ead6-d3fd-4d00-ad40-f70d85b4b7e9`

#### 3. **uofi-itp-directory-data** (Data Layer)
- **Type**: Class library
- **Purpose**: Database context, models, and data access logic
- **Key Dependencies**:
  - Microsoft.EntityFrameworkCore.SqlServer (SQL Server provider)
  - Microsoft.EntityFrameworkCore.Tools (Migration tools)
  - Azure.Storage.Blobs (Azure Blob Storage for images/documents)
  - Newtonsoft.Json (JSON serialization)
- **Key Components**:
  - DirectoryContext (EF Core DbContext)
  - Repository pattern implementations
  - Data models and entities
  - Helper classes for data operations
  - Migration files

#### 4. **uofi-itp-directory-search** (Search Layer)
- **Type**: Class library
- **Purpose**: Amazon OpenSearch Service integration and search logic
- **Key Dependencies**:
  - OpenSearch.Client (OpenSearch operations)
  - OpenSearch.Net.Auth.AwsSigV4 (AWS authentication)
  - AWSSDK.OpenSearchService (AWS SDK)
- **Key Components**:
  - Search indexing and mapping
  - Query builders
  - Search result models
  - SearchStax integration

#### 5. **uofi-itp-directory-external** (External Integrations)
- **Type**: Class library
- **Purpose**: Connections to external campus systems
- **Key Dependencies**:
  - Newtonsoft.Json
- **External Systems**:
  - Enterprise Data Warehouse (EDW)
  - Program Course Repository
  - Illinois Experts API
  - Email services (Socket Labs)

## Local Development Setup

### Prerequisites

- **Visual Studio 2022** or later (or VS Code with C# DevKit)
- **.NET 8 SDK**
- **SQL Server** (LocalDB, Developer Edition, or Express)
- **Azure Storage Emulator** or Azure Storage Account (for blob storage)
- **VPN Access** to University of Illinois network (required for Data Warehouse access)
- **Git** for version control

### Step-by-Step Setup

#### 1. Clone the Repository
```bash
git clone https://github.com/web-illinois/data-directory
cd data-directory
```

#### 2. Restore NuGet Packages
```bash
dotnet restore
```
Or in Visual Studio: Right-click solution → **Restore NuGet Packages**

#### 3. Configure Application Secrets

The solution uses **User Secrets** for sensitive configuration. You need to configure secrets for both the Blazor app and the Function app.

##### For the Blazor Application (uofi-itp-directory)

Navigate to the project directory and run:
```bash
cd uofi-itp-directory
dotnet user-secrets set "ConnectionStrings:AppConnection" "Server=(localdb)\\mssqllocaldb;Database=DirectoryDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "DataWarehouseUrl" "YOUR_DATA_WAREHOUSE_URL"
dotnet user-secrets set "DataWarehouseKey" "YOUR_DATA_WAREHOUSE_KEY"
dotnet user-secrets set "ExpertsUrl" "YOUR_EXPERTS_API_URL"
dotnet user-secrets set "ExpertsSecretKey" "YOUR_EXPERTS_API_KEY"
dotnet user-secrets set "ProgramCourseUrl" "YOUR_PROGRAM_COURSE_URL"
dotnet user-secrets set "FacultyLoadUrl" "YOUR_FACULTY_LOAD_URL"
dotnet user-secrets set "AzureStorage" "UseDevelopmentStorage=true"
dotnet user-secrets set "AzureAccountName" "devstoreaccount1"
dotnet user-secrets set "AzureAccountKey" "YOUR_AZURE_STORAGE_KEY"
dotnet user-secrets set "AzureImageContainerName" "images"
dotnet user-secrets set "AzureCvContainerName" "cvs"
dotnet user-secrets set "WebServicesSignatureLink" "YOUR_SIGNATURE_SERVICE_URL"
```

##### For the Azure Functions (uofi-itp-directory-function)

Edit `uofi-itp-directory-function/local.settings.json`:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AppConnection": "Server=(localdb)\\mssqllocaldb;Database=DirectoryDb;Trusted_Connection=True;TrustServerCertificate=True",
    "SearchUrl": "YOUR_OPENSEARCH_URL",
    "AccessKey": "YOUR_AWS_ACCESS_KEY",
    "SecretKey": "YOUR_AWS_SECRET_KEY",
    "SearchStaxUrl": "YOUR_SEARCHSTAX_URL",
    "SearchStaxApiToken": "YOUR_SEARCHSTAX_TOKEN",
    "DataWarehouseUrl": "YOUR_DATA_WAREHOUSE_URL",
    "DataWarehouseKey": "YOUR_DATA_WAREHOUSE_KEY",
    "ExpertsUrl": "YOUR_EXPERTS_API_URL",
    "ExpertsSecretKey": "YOUR_EXPERTS_API_KEY",
    "ProgramCourseUrl": "YOUR_PROGRAM_COURSE_URL",
    "FacultyLoadUrl": "YOUR_FACULTY_LOAD_URL",
    "SocketApiKey": "YOUR_EMAIL_API_KEY"
  },
  "Host": {
    "LocalHttpPort": 7207,
    "CORS": "*"
  }
}
```

**Note**: `local.settings.json` is ignored by git for security.

#### 4. Set Up the Database

The database will be created automatically using Entity Framework Core migrations when the application first runs. However, you can manually apply migrations:

##### Set Startup Project
In Visual Studio:
- Right-click **uofi-itp-directory** project → **Set as Startup Project**

##### Run Migrations
Open **Package Manager Console** in Visual Studio:
```powershell
Update-Database -Project uofi-itp-directory-data
```

Or using CLI:
```bash
dotnet ef database update --project uofi-itp-directory-data --startup-project uofi-itp-directory
```

**Initial Setup**: The database will automatically seed with Rob and Bryan as admin users.

#### 5. Set Up Amazon OpenSearch Service

1. Create an OpenSearch domain through AWS Console or AWS CLI
2. Note the endpoint URL
3. Configure AWS credentials (Access Key and Secret Key)
4. Set the mapping through the Function App HTTP request **LoadMapping** endpoint

#### 6. Configure Azure AD Authentication (Optional for Local Dev)

The `appsettings.json` contains Azure AD configuration. For local development, you may need to:
- Register an app in Azure AD
- Update `ClientId` and `TenantId` in `appsettings.json` or user secrets
- Or bypass authentication for local testing

#### 7. Run the Applications

##### Option A: Run Both Projects Simultaneously
In Visual Studio:
1. Right-click solution → **Properties**
2. Select **Multiple startup projects**
3. Set both `uofi-itp-directory` and `uofi-itp-directory-function` to **Start**
4. Press **F5** to run

##### Option B: Run Individually
```bash
# Terminal 1 - Blazor App
cd uofi-itp-directory
dotnet run

# Terminal 2 - Function App
cd uofi-itp-directory-function
dotnet run
```

The Blazor app will typically run on `https://localhost:5001` and the Function app on `http://localhost:7207`.

### Required External Access

- **University of Illinois VPN**: Required to access the Enterprise Data Warehouse, even when testing locally
- **AWS Account**: For OpenSearch service
- **Azure Storage Account**: For blob storage (images, CVs, documents)

## Entity Framework Core Migrations

### Creating a New Migration

**Important**: Set `uofi-itp-directory` as the startup project before running migration commands.

Using Package Manager Console:
```powershell
Add-Migration -Name YourMigrationName -Project uofi-itp-directory-data
```

Using CLI:
```bash
dotnet ef migrations add YourMigrationName --project uofi-itp-directory-data --startup-project uofi-itp-directory
```

### Applying Migrations

Using Package Manager Console:
```powershell
Update-Database -Project uofi-itp-directory-data
```

Using CLI:
```bash
dotnet ef database update --project uofi-itp-directory-data --startup-project uofi-itp-directory
```

### Rolling Back a Migration

```bash
dotnet ef database update PreviousMigrationName --project uofi-itp-directory-data --startup-project uofi-itp-directory
```

## Deployment

### CI/CD Pipeline

Deployments use **GitHub Actions** for continuous integration and deployment:
- **Function App**: Deploys to Azure Functions
- **Web App**: Deploys to Azure App Service

### Deployment Targets
- **Production**: Automated deployment on merge to main branch
- **Staging**: Automated deployment on merge to staging branch

## Troubleshooting

### Common Issues and Solutions

#### 1. "The certificate chain was issued by an authority that is not trusted"

**Cause**: SQL Server certificate validation issue in local development.

**Solution**: Add `TrustServerCertificate=True` to your connection string:
```
Server=(localdb)\\mssqllocaldb;Database=DirectoryDb;Trusted_Connection=True;TrustServerCertificate=True
```

#### 2. Database Migration Fails

**Symptoms**: Migration commands fail or database isn't created.

**Solutions**:
- Ensure `uofi-itp-directory` is set as the startup project
- Verify connection string in user secrets or local.settings.json
- Check that SQL Server is running
- Try deleting the database and running `Update-Database` again
- Check for syntax errors in migration files

#### 3. Cannot Access Data Warehouse

**Cause**: Not connected to University of Illinois VPN.

**Solution**: 
- Connect to the University of Illinois VPN
- Verify VPN connection is active
- Test connectivity to the Data Warehouse URL

#### 4. Azure Function Not Starting Locally

**Symptoms**: Function app fails to start or shows configuration errors.

**Solutions**:
- Verify all required settings in `local.settings.json`
- Check that Azure Storage Emulator is running (if using local storage)
- Ensure port 7207 is not in use by another application
- Check Application Insights connection (can be disabled for local dev)

#### 5. Authentication Failures (Azure AD)

**Symptoms**: Redirect loops, authentication errors, or unauthorized access.

**Solutions**:
- Verify Azure AD app registration settings
- Check `ClientId` and `TenantId` match your Azure AD app
- Ensure redirect URIs are configured correctly in Azure AD
- Clear browser cookies and cache
- For local dev, consider temporarily bypassing authentication

#### 6. Blob Storage Errors

**Symptoms**: Image upload fails, CV upload errors.

**Solutions**:
- Verify Azure Storage Emulator is running (for local dev)
- Check connection string format
- Ensure container names exist and are correctly configured
- Verify storage account keys are correct

#### 7. OpenSearch Connection Issues

**Symptoms**: Search functionality not working, connection timeouts.

**Solutions**:
- Verify OpenSearch endpoint URL is accessible
- Check AWS credentials (Access Key and Secret Key)
- Ensure security group/firewall allows access to OpenSearch
- Verify the index exists (use LoadMapping endpoint to create)
- Check OpenSearch service health in AWS Console

#### 8. "PersonMapper is null" on Function App Startup

**Cause**: Dependency injection configuration issue.

**Solution**: 
- Verify all dependencies in `Program.cs` are properly registered
- Check that OpenSearch credentials are configured
- Ensure database connection string is valid

#### 9. CORS Errors Between Blazor App and Function App

**Symptoms**: API calls from Blazor to Function fail with CORS errors.

**Solutions**:
- Verify CORS is set to `"*"` in `local.settings.json` (Host section)
- Check that both apps are running on expected ports
- Ensure function app URL is correctly configured in Blazor app settings

#### 10. Build Errors After Pulling Latest Code

**Solutions**:
- Clean and rebuild solution: `dotnet clean` then `dotnet build`
- Restore NuGet packages: `dotnet restore`
- Delete `bin` and `obj` folders manually
- Check for breaking changes in recent commits
- Verify .NET 8 SDK is installed

### Logging and Diagnostics

- **Application Insights**: Configured in function app for production telemetry
- **Console Logging**: Available in both applications during local development
- **EF Core Logging**: Enabled with `EnableSensitiveDataLogging(true)` in development

### Getting Help

For additional support:
1. Check the GitHub repository issues
2. Review commit history for recent changes
3. Contact the development team (Rob and Bryan are default admins)
4. Consult Azure Application Insights for production errors

## Additional Notes

### Key Configuration Settings

Both applications require several external service configurations:
- **SQL Server**: Database connection
- **Azure AD**: Authentication
- **Data Warehouse**: Employee data source
- **Illinois Experts API**: Faculty research/publications
- **Program Course Repository**: Course information
- **OpenSearch/SearchStax**: Search functionality
- **Azure Blob Storage**: File storage
- **Email Service (Socket Labs)**: Notifications

### Security Considerations

- Never commit `local.settings.json` or user secrets
- Use User Secrets for local development
- Use Azure Key Vault or App Settings for production
- Ensure all API keys are rotated regularly
- Review Azure AD permissions carefully

### Performance Notes

- The system uses caching (CacheHolder) for frequently accessed data
- OpenSearch indexing runs asynchronously
- Image uploads are processed and scaled using SixLabors.ImageSharp
- Database queries use EF Core with eager loading where appropriate

---

**Last Updated**: 2024  
**Maintainers**: IT Partners Team  
**Target Framework**: .NET 8  
**License**: University of Illinois