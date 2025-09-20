using Autofac;
using Autofac.Extensions.DependencyInjection;
using Entegro;
using Entegro.Application.Mappings;
using Entegro.Engine;
using Entegro.Infrastructure.Data;
using Entegro.Infrastructure.Extensions;
using Entegro.Utilities;
using Entegro.Web.Hubs;
using Entegro.Web.Mappings;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
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

#region MVC + JSON
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
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

#region App Services
builder.Services.AddApplicationServices();
builder.Services.AddRepositoryServices();
builder.Services.AddCommerceServices();
builder.Services.AddErpServices();
builder.Services.AddMarketplaceServices();

MapsterConfig.RegisterMappings();
WebMapsterConfig.RegisterMappings();
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddSignalR();
#endregion

#region Build App
var app = builder.Build();

RotativaConfiguration.Setup(builder.Environment.WebRootPath);

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
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EntegroDbContext>();
    await db.Database.MigrateAsync();
}

engineStarter.ConfigureApplication(app);

app.MapHub<NotificationHub>("/notificationHub");
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