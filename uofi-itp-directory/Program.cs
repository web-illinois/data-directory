using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DirectoryHook;
using uofi_itp_directory_data.Helpers;
using uofi_itp_directory_data.Security;
using uofi_itp_directory_data.Uploads;
using uofi_itp_directory_external.DataWarehouse;
using uofi_itp_directory_external.Experts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddControllersWithViews()
.AddMicrosoftIdentityUI();

builder.Services.AddDbContextFactory<DirectoryContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")).EnableSensitiveDataLogging(true));

builder.Services.AddScoped<DirectoryRepository>();
builder.Services.AddScoped(c => new DirectoryHookHelper(c.GetService<DirectoryRepository>(), builder.Configuration["FacultyLoadUrl"]));
builder.Services.AddScoped<LogHelper>();
builder.Services.AddScoped(c => new DataWarehouseManager(builder.Configuration["DataWarehouseUrl"], builder.Configuration["DataWarehouseKey"]));
builder.Services.AddScoped(c => new IllinoisExpertsManager(builder.Configuration["ExpertsUrl"], builder.Configuration["ExpertsSecretKey"]));
builder.Services.AddScoped<PersonOptionHelper>();
builder.Services.AddScoped<AreaHelper>();
builder.Services.AddScoped<LookupHelper>();
builder.Services.AddScoped<OfficeHelper>();
builder.Services.AddScoped<EmployeeHelper>();
builder.Services.AddScoped<JobProfileHelper>();
builder.Services.AddScoped<SecurityEntryHelper>();
builder.Services.AddScoped<EmployeeAreaHelper>();
builder.Services.AddScoped<OfficeManagerHelper>();
builder.Services.AddScoped<ImageScaler>();
builder.Services.AddScoped<EmployeeActivityHelper>();
builder.Services.AddScoped(b => new UploadStorage(builder.Configuration["AzureStorage"], builder.Configuration["AzureAccountName"], builder.Configuration["AzureAccountKey"], builder.Configuration["AzureImageContainerName"], builder.Configuration["AzureCvContainerName"]));
builder.Services.AddScoped(b => new SignatureGenerator(builder.Configuration["WebServicesSignatureLink"]));
builder.Services.AddSingleton<CacheHolder>();

builder.Services.AddAuthorization(options => {
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Lifetime.ApplicationStarted.Register(() => {
    var factory = app.Services.GetService<IServiceScopeFactory>() ?? throw new NullReferenceException("service scope factory is null");
    using var serviceScope = factory.CreateScope();
    var context = serviceScope.ServiceProvider.GetRequiredService<DirectoryContext>();
    context.Database.Migrate();
});

app.Run();