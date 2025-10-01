using Autofac;
using Autofac.Extensions.DependencyInjection;
using Entegro;
using Entegro.Api.Jobs;
using Entegro.Api.Services;
using Entegro.Application.Mappings;
using Entegro.Engine;
using Entegro.Infrastructure.Data;
using Entegro.Infrastructure.Extensions;
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
using Serilog.Sinks.MSSqlServer;
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
var columnOptions = new ColumnOptions();
var sinkOptions = new MSSqlServerSinkOptions
{
    TableName = "Log",             // tablo adý
    AutoCreateSqlTable = true,      // tablo yoksa oluþtur
    BatchPostingLimit = 50,         // her seferinde 50 log gönder
    BatchPeriod = TimeSpan.FromSeconds(5) // 5 saniyede bir batch gönder
};
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: sinkOptions,
        columnOptions: columnOptions,
        appConfiguration: builder.Configuration
    )
    .Enrich.FromLogContext()
    .CreateLogger();

Serilog.Debugging.SelfLog.Enable(msg =>
{
    Console.WriteLine("Serilog Error: " + msg);
});


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
builder.Services.AddQuartz();
builder.Services.AddHostedService<QuartzSchedulerService>();
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
