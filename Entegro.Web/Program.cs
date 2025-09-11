using Autofac;
using Autofac.Extensions.DependencyInjection;
using Entegro;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings;
using Entegro.Application.Services;
using Entegro.Application.Services.Commerce;
using Entegro.Application.Services.Commerce.Smartstore;
using Entegro.Application.Services.Marketplace;
using Entegro.Domain.Entities.Catalog;
using Entegro.Engine;
using Entegro.Infrastructure.Data;
using Entegro.Infrastructure.EventBus;
using Entegro.Infrastructure.Repositories;
using Entegro.Utilities;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
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
var baseDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = isDevEnvironment ? null : baseDirectory
});

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
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

builder.Services.AddDbContext<EntegroDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();
});
var configuration = (IConfiguration)builder.Configuration;


builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .UseSerilog(dispose: true);


var startupLogger = new SerilogLoggerFactory(Log.Logger).CreateLogger("File");
var appContext = new SmartApplicationContext(builder.Environment, configuration, startupLogger);
var engine = EngineFactory.Create(appContext.AppConfiguration);
var engineStarter = engine.Start(appContext);

// Configure RequestSizeLimit and RequestFormLimits
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

// Add NativeLibraryDirectory to PATH environment variable
AddPathToEnv(appContext.RuntimeInfo.NativeLibraryDirectory);

// Add services to the container.
engineStarter.ConfigureServices(builder.Services);

// Add services to the Autofac container.
builder.Host.ConfigureContainer<ContainerBuilder>(engineStarter.ConfigureContainer);

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IProductCategoryMappingRepository, ProductCategoryMappingRepository>();
builder.Services.AddScoped<IProductCategoryMappingService, ProductCategoryMappingService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();

builder.Services.AddScoped<IMediaFolderRepository, MediaFolderRepository>();
builder.Services.AddScoped<IMediaFolderService, MediaFolderService>();


builder.Services.AddScoped<IMediaFileRepository, MediaFileRepository>();
builder.Services.AddScoped<IMediaFileService, MediaFileService>();


builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
builder.Services.AddScoped<IProductAttributeService, ProductAttributeService>();

builder.Services.AddScoped<IProductAttributeValueRepository, ProductAttributeValueRepository>();
builder.Services.AddScoped<IProductAttributeValueService, ProductAttributeValueService>();

builder.Services.AddScoped<IProductVariantAttributeRepository, ProductVariantAttributeRepository>();
builder.Services.AddScoped<IProductVariantAttributeService, ProductVariantAttributeService>();

builder.Services.AddScoped<IProductVariantAttributeValueRepository, ProductVariantAttributeValueRepository>();
builder.Services.AddScoped<IProductVariantAttributeValueService, ProductVariantAttributeValueService>();

builder.Services.AddScoped<IProductAttributeFormatter, ProductAttributeFormatter>();

builder.Services.AddScoped<IProductImageMappingRepository, ProductImageMappingRepository>();
builder.Services.AddScoped<IProductImageMappingService, ProductImageMappingService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddScoped<IProductVariantAttributeCombinationService, ProductVariantAttributeCombinationService>();
builder.Services.AddScoped<IProductVariantAttributeCombinationRepository, ProductVariantAttributeCombinationRepository>();

builder.Services.AddScoped<IIntegrationSystemRepository, IntegrationSystemRepository>();
builder.Services.AddScoped<IIntegrationSystemService, IntegrationSystemService>();

builder.Services.AddScoped<IIntegrationSystemParameterRepository, IntegrationSystemParameterRepository>();
builder.Services.AddScoped<IIntegrationSystemParameterService, IntegrationSystemParameterService>();

builder.Services.AddScoped<IIntegrationSystemLogRepository, IntegrationSystemLogRepository>();
builder.Services.AddScoped<IIntegrationSystemLogService, IntegrationSystemLogService>();


builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryService, CountryService>();


builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICityService, CityService>();

builder.Services.AddScoped<ITownRepository, TownRepository>();
builder.Services.AddScoped<ITownService, TownService>();

builder.Services.AddScoped<IOrderNoteRepository, OrderNoteRepository>();
builder.Services.AddScoped<IOrderNoteService, OrderNoteService>();


builder.Services.AddScoped<IProductIntegrationRepository, ProductIntegrationRepository>();
builder.Services.AddScoped<IProductIntegrationService, ProductIntegrationService>();


builder.Services.AddScoped<ISpecificationAttributeOptionRepository, SpecificationAttributeOptionRepository>();
builder.Services.AddScoped<ISpecificationAttributeOptionService, SpecificationAttributeOptionService>();


builder.Services.AddScoped<ISpecificationAttributeRepository, SpecificationAttributeRepository>();
builder.Services.AddScoped<ISpecificationAttributeService, SpecificationAttributeService>();


builder.Services.AddScoped<IProductSpecificationAttributeMappingRepository, ProductSpecificationAttributeMappingRepository>();
builder.Services.AddScoped<IProductSpecificationAttributeMappingService, ProductSpecificationAttributeMappingService>();


builder.Services.AddScoped<IEmailAccountRepository, EmailAccountRepository>();
builder.Services.AddScoped<IEmailAccountService, EmailAccountService>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();

builder.Services.AddScoped<ITreeNodeRepository<Category>, TreeNodeRepository<Category>>();


builder.Services.AddScoped<ISmartstoreService, SmartstoreService>();


builder.Services.AddScoped<IEventPublisher, EventBus>();
builder.Services.AddScoped<SmartstoreClient>();

builder.Services.AddScoped<ICommerceProductWriter, SmartstoreProductWriter>();
builder.Services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, SmartstoreProductWriter>();

builder.Services.AddScoped<ITrendyolService, TrendyolService>();
builder.Services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, TrendyolService>();

builder.Services.AddScoped<IN11Service, N11Service>();
builder.Services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, N11Service>();

builder.Services.AddHttpClient();

MapsterConfig.RegisterMappings();
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

var app = builder.Build();
var providerContainer = appContext as IServiceProviderContainer;
providerContainer.ApplicationServices = app.Services;

// At this stage we can set the scoped service container.
engine.Scope = new ScopedServiceContainer(
    app.Services.GetRequiredService<ILifetimeScopeAccessor>(),
    app.Services.GetRequiredService<IHttpContextAccessor>(),
    app.Services.AsLifetimeScope());

// Build request pipeline
app.Lifetime.ApplicationStarted.Register(() =>
{
    appContext.Freeze();
    engineStarter.Dispose();
    engineStarter = null;
});

var supportedCultures = new[] { new CultureInfo("en-US") };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

app.Run();


bool IsDevEnvironment()
{
    if (environmentName == Environments.Development)
        return true;

    if (System.Diagnostics.Debugger.IsAttached)
        return true;

    // if there's a 'Smartstore.sln' in one of the parent folders,
    // then we're likely in a dev environment
    if (CommonHelper.FindSolutionRoot(Directory.GetCurrentDirectory()) != null)
        return true;

    return false;
}
void AddPathToEnv(string path)
{
    var name = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Path" : "PATH";
    var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

    if (value.IsEmpty() || !value.Contains(path))
    {
        value = value.EmptyNull().Trim(';') + ';' + path;
        Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
    }
}