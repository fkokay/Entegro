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
using Entegro.Infrastructure.Data;
using Entegro.Infrastructure.Repositories;
using Entegro.Service;
using Entegro.Service.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<EntegroContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IProductIntegrationRepository, ProductIntegrationRepository>();
builder.Services.AddScoped<IProductIntegrationService, ProductIntegrationService>();

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IMediaFileRepository, MediaFileRepository>();
builder.Services.AddScoped<IMediaFileService, MediaFileService>();

builder.Services.AddScoped<IMediaFolderRepository, MediaFolderRepository>();
builder.Services.AddScoped<IMediaFolderService, MediaFolderService>();

builder.Services.AddScoped<ISmartstoreService, SmartstoreService>();
builder.Services.AddScoped<ITrendyolService, TrendyolService>();
builder.Services.AddScoped<IErpService, ErpService>();

builder.Services.AddScoped<SmartstoreClient>();
builder.Services.AddScoped<ICommerceProductWriter, SmartstoreProductWriter>();
builder.Services.AddScoped<ICommerceBrandWriter, SmartstoreManufacturerWriter>();
builder.Services.AddScoped<ICommerceCategoryWriter, SmartstoreCategoryWriter>();
builder.Services.AddHttpClient();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

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

    //var jobKeyErp = new JobKey("ErpDataSyncJob");

    //q.AddJob<ErpDataSyncJob>(opts => opts.WithIdentity(jobKeyErp));

    //q.AddTrigger(opts => opts
    //    .ForJob(jobKeyErp)
    //    .WithIdentity("ErpDataSyncJob-trigger")
    //    .WithSimpleSchedule(x => x
    //        .WithIntervalInMinutes(10)
    //        .RepeatForever())
    //);
});

// Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();
