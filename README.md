# IT Partners README.MD file

## Summary: 

Back-end and APIs to pull information. This is replacing the old people function app and the old contact support app.

## Production location:

There will be two locations, one for each project:
* https://directoryapi.wigg.illinois.edu. This has a database connection, connection to AWS OpenSearch Service, and is only used to load and serve area, office, employee, and directory information. 
* https://directory.wigg.illinois.edu. This also has a database connection and is a Blazor application. 

## Development location: 

Currently, none. We do development on local machines.

## How to deploy to production/development: 

Using CI -- Github Action to a Function App and an App Service. 

## How to set up locally: 

Download the code. Use NuGet to get the latest copy of the executables. 

The "local.settings.json" needs to be filled in with a proper Data Warehouse key and Experts API information, or add it to your local secrets file. 

The "local.settings.json" for the Directory Function App needs to be filled in

```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
    }
}
```

The database uses EF to generate, and it will generate Rob and Bryan as admins automatically. The AWS OpenSearch Service is automatically generated through the API. 

## Notes (error logging, external tools, links, etc.): 

Information about EF Core Tools:

Make sure the uofi-itp-directory project is set up as the startup project before running the commands below:

``Add-Migration -Name {migration name} -Project uofi-itp-directory-data``

``Update-Database -Project uofi-itp-directory-data``

If you run into the issue "The certificate chain was issued by an authority that is not trusted.", then add **TrustServerCertificate=True** to the connection string.

There are five projects in this solution. They are:
* uofi-itp-directory: The Blazor Application
* uofi-itp-directory-function: The Function application
* uofi-itp-directory-data: A class project that contains the database and logic to add/retrieve from the database
* uofi-itp-directory-search: A class project that contains the Amazon Open Search Services and logic to add/retrieve from the service
* uofi-itp-directory-external: A class project that contains connections to campus solutions (Data Warehouse, Program Course Repository, Illinois Experts)

### Rebuilding the AWS OpenSearch Service

If you need to rebuild the AWS OpenSearch Service because the maaping needs changed, you will need to transfer the data. To do so, follow the steps below:

1. Get access to the OpenSearch Service through Tech Services. You will need a fixed IP address for this. 
2. Run `POST _reindex {  "source": { "index":"dr_person" }, "dest":{ "index":"dr_person2"  } }`
3. Run `DELETE /dr_person`
4. Deploy the code. This will rebuild the index when the API runs.
5. Run `POST _reindex {  "source": { "index":"dr_person2" }, "dest":{ "index":"dr_person"  } }`
6. Run `DELETE /dr_person2`
