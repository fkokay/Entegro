using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Entegro;
using Entegro.Application.Events;
using Entegro.Application.Interfaces;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Interfaces.Services.Erp;
using Entegro.Application.Interfaces.Services.Marketplace;
using Entegro.Application.Mappings;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Entegro.Application.Services;
using Entegro.Application.Services.Commerce;
using Entegro.Application.Services.Commerce.Smartstore;
using Entegro.Application.Services.Erp;
using Entegro.Application.Services.Marketplace;
using Entegro.Engine;
using Entegro.Infrastructure.Data;
using Entegro.Infrastructure.EventBus;
using Entegro.Infrastructure.Repositories;
using Entegro.Service;
using Entegro.Service.Jobs;
using Entegro.Utilities;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using Serilog.Extensions.Logging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

var rgSystemSource = new Regex("^File|^System|^Microsoft|^Serilog|^Autofac|^Castle|^MiniProfiler|^Newtonsoft|^Pipelines|^Azure|^StackExchange|^Superpower|^Dasync", RegexOptions.Compiled);
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production;
var isDevEnvironment = IsDevEnvironment();
var baseDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();


var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .UseSerilog(dispose: true)
    .ConfigureContainer<ContainerBuilder>((hostContext, containerBuilder) =>
    {
        var configuration = hostContext.Configuration;
        var environment = hostContext.HostingEnvironment;

        var startupLogger = new SerilogLoggerFactory(Log.Logger).CreateLogger("File");
        var appContext = new SmartApplicationContext(environment, configuration, startupLogger);
        var engine = EngineFactory.Create(appContext.AppConfiguration);
        var engineStarter = engine.Start(appContext);

        engineStarter.ConfigureContainer(containerBuilder);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        var environment = hostContext.HostingEnvironment;

        var startupLogger = new SerilogLoggerFactory(Log.Logger).CreateLogger("File");
        var appContext = new SmartApplicationContext(environment, configuration, startupLogger);
        var engine = EngineFactory.Create(appContext.AppConfiguration);
        var engineStarter = engine.Start(appContext);

        AddPathToEnv(appContext.RuntimeInfo.NativeLibraryDirectory);

        engineStarter.ConfigureServices(services);


        services.AddDbContext<EntegroContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        MapsterConfig.RegisterMappings();
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddHttpClient();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductIntegrationRepository, ProductIntegrationRepository>();
        services.AddScoped<IProductIntegrationService, ProductIntegrationService>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
        services.AddScoped<IProductAttributeService, ProductAttributeService>();
        services.AddScoped<IProductAttributeValueRepository, ProductAttributeValueRepository>();
        services.AddScoped<IProductAttributeValueService, ProductAttributeValueService>();
        services.AddScoped<IProductVariantAttributeCombinationRepository, ProductVariantAttributeCombinationRepository>();
        services.AddScoped<IProductVariantAttributeCombinationService, ProductVariantAttributeCombinationService>();
        services.AddScoped<IProductVariantAttributeRepository, ProductVariantAttributeRepository>();
        services.AddScoped<IProductVariantAttributeService, ProductVariantAttributeService>();
        services.AddScoped<IProductVariantAttributeValueRepository, ProductVariantAttributeValueRepository>();
        services.AddScoped<IProductVariantAttributeValueService, ProductVariantAttributeValueService>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IMediaFileRepository, MediaFileRepository>();
        services.AddScoped<IMediaFileService, MediaFileService>();
        services.AddScoped<IMediaFolderRepository, MediaFolderRepository>();
        services.AddScoped<IMediaFolderService, MediaFolderService>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IAddressService, AddressService>();

        services.AddScoped<IEventPublisher, EventBus>();
        services.AddScoped<SmartstoreClient>();
        services.AddScoped<ISmartstoreService, SmartstoreService>();
        services.AddScoped<ICommerceProductWriter, SmartstoreProductWriter>();
        services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, SmartstoreProductWriter>();
        services.AddScoped<ICommerceBrandWriter, SmartstoreManufacturerWriter>();
        services.AddScoped<ICommerceCategoryWriter, SmartstoreCategoryWriter>();
        services.AddScoped<ITrendyolService, TrendyolService>();
        services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, TrendyolService>();
        services.AddScoped<IN11Service, N11Service>();
        services.AddScoped<IEventHandler<ProductIntegrationRecordUpdatedEvent>, N11Service>();
        services.AddScoped<IErpService, ErpService>();

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        services.AddQuartz(q =>
        {

            var jobKeySmartstore = new JobKey("SmartstoreDataSyncJob");

            q.AddJob<SmartstoreDataSyncJob>(opts => opts.WithIdentity(jobKeySmartstore));

            q.AddTrigger(opts => opts
                .ForJob(jobKeySmartstore)
                .WithIdentity("SmartstoreDataSyncJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                );

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
    }).Build();

host.Run();


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