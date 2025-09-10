using Autofac;
using Autofac.Extensions.DependencyInjection;
using Entegro;
using Entegro.Api.Jobs;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Mappings;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Entegro.Application.Services;
using Entegro.Application.Services.Commerce;
using Entegro.Engine;
using Entegro.Infrastructure.Data;
using Entegro.Infrastructure.Extensions;
using Entegro.Infrastructure.Repositories;
using Entegro.Utilities;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


var rgSystemSource = new Regex("^File|^System|^Microsoft|^Serilog|^Autofac|^Castle|^MiniProfiler|^Newtonsoft|^Pipelines|^Azure|^StackExchange|^Superpower|^Dasync", RegexOptions.Compiled);
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production;
var isDevEnvironment = IsDevEnvironment();
var baseDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = isDevEnvironment ? null : baseDirectory
});

#region Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.Graylog(new GraylogSinkOptions
    {
        HostnameOrAddress = "127.0.0.1",
        Port = 12201,
        Facility = "EntegroWebApp",
        TransportType = TransportType.Udp
    })
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

#region MVC
builder.Services.AddControllers();
builder.Services.AddOpenApi();
#endregion

#region Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.Cookie.Name = "EntegroAuthentication";
    options.CookieManager = new ChunkingCookieManager();
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.LoginPath = "/Identity/Login";
    options.LogoutPath = "/Identity/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.MaxAge = options.ExpireTimeSpan;
});
builder.Services.AddAuthorization();
#endregion

#region EF Core
builder.Services.AddDbContext<EntegroDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();
});
#endregion

#region Autofac & Engine
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
             .UseSerilog(dispose: true);

var startupLogger = new SerilogLoggerFactory(Log.Logger).CreateLogger("File");
var appContext = new SmartApplicationContext(builder.Environment, builder.Configuration, startupLogger);
var engine = EngineFactory.Create(appContext.AppConfiguration);
var engineStarter = engine.Start(appContext);

if (appContext.AppConfiguration.MaxRequestBodySize != null)
{
    builder.WebHost.ConfigureKestrel(kestrel =>
    {
        kestrel.Limits.MaxRequestBodySize = appContext.AppConfiguration.MaxRequestBodySize;
    });
    builder.Services.Configure<FormOptions>(form =>
    {
        form.MultipartBodyLengthLimit = appContext.AppConfiguration.MaxRequestBodySize.Value;
    });
}
AddPathToEnv(appContext.RuntimeInfo.NativeLibraryDirectory);

engineStarter.ConfigureServices(builder.Services);
builder.Host.ConfigureContainer<ContainerBuilder>(engineStarter.ConfigureContainer);
#endregion

#region Swagger
builder.Services.AddSwaggerGen();
#endregion

#region Jobs
builder.Services.AddQuartz(q =>
{
    //var jobKeySmartstore = new JobKey("SmartstoreDataSyncJob");

    //q.AddJob<SmartstoreDataSyncJob>(opts => opts.WithIdentity(jobKeySmartstore));

    //q.AddTrigger(opts => opts
    //    .ForJob(jobKeySmartstore)
    //    .WithIdentity("SmartstoreDataSyncJob-trigger")
    //    .WithSimpleSchedule(x => x
    //        .WithIntervalInMinutes(1)
    //        .RepeatForever())
    //    );

    //var jobKeyTrendyol = new JobKey("TrendyolDataSyncJob");

    //q.AddJob<TrendyolDataSyncJob>(opts => opts.WithIdentity(jobKeyTrendyol));

    //q.AddTrigger(opts => opts
    //    .ForJob(jobKeyTrendyol)
    //    .WithIdentity("TrendyolDataSyncJob-trigger")
    //    .WithSimpleSchedule(x => x
    //        .WithIntervalInMinutes(10)
    //        .RepeatForever())
    //);

    var jobKeyErp = new JobKey("ErpDataSyncJob");

    q.AddJob<ErpDataSyncJob>(opts => opts.WithIdentity(jobKeyErp));

    q.AddTrigger(opts => opts
        .ForJob(jobKeyErp)
        .WithIdentity("ErpDataSyncJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(10)
            .RepeatForever())
        );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
#endregion

#region App Services
builder.Services.AddApplicationServices();
builder.Services.AddRepositoryServices();
builder.Services.AddCommerceServices();
builder.Services.AddErpServices();
builder.Services.AddMarketplaceServices();

MapsterConfig.RegisterMappings();
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();
#endregion

#region Build App
var app = builder.Build();
(appContext as IServiceProviderContainer)!.ApplicationServices = app.Services;

engine.Scope = new ScopedServiceContainer(
    app.Services.GetRequiredService<ILifetimeScopeAccessor>(),
    app.Services.GetRequiredService<IHttpContextAccessor>(),
    app.Services.AsLifetimeScope());

app.Lifetime.ApplicationStarted.Register(() =>
{
    appContext.Freeze();
    engineStarter.Dispose();
});

var supportedCultures = new[] { new CultureInfo("en-US") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (!app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

engineStarter.ConfigureApplication(app);
app.Run();
#endregion

#region Helpers
bool IsDevEnvironment()
{
    if (environmentName == Environments.Development) return true;
    if (System.Diagnostics.Debugger.IsAttached) return true;
    if (CommonHelper.FindSolutionRoot(Directory.GetCurrentDirectory()) != null) return true;
    return false;
}
void AddPathToEnv(string path)
{
    var name = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Path" : "PATH";
    var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    if (string.IsNullOrWhiteSpace(value) || !value.Contains(path))
    {
        value = (value ?? "").Trim(';') + ';' + path;
        Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
    }
}
#endregion
