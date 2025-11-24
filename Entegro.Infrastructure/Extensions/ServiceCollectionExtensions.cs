using Entegro.Application.Interfaces.Event;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Cargo;
using Entegro.Application.Services.Base;
using Entegro.Application.Services.Cargo;
using Entegro.Infrastructure.Messaging;
using Entegro.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Entegro.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IMediaFolderService, MediaFolderService>();
            services.AddScoped<IMediaFileService, MediaFileService>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductAttributeValueService, ProductAttributeValueService>();
            services.AddScoped<IProductVariantAttributeService, ProductVariantAttributeService>();
            services.AddScoped<IProductVariantAttributeValueService, ProductVariantAttributeValueService>();
            services.AddScoped<IProductVariantAttributeCombinationService, ProductVariantAttributeCombinationService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IProductAttributeFormatter, ProductAttributeFormatter>();
            services.AddScoped<IProductMediaFileMappingService, ProductMediaFileMappingService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IIntegrationSystemService, IntegrationSystemService>();
            services.AddScoped<IIntegrationSystemParameterService, IntegrationSystemParameterService>();
            services.AddScoped<IIntegrationSystemLogService, IntegrationSystemLogService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<ITownService, TownService>();
            services.AddScoped<IOrderNoteService, OrderNoteService>();
            services.AddScoped<IProductIntegrationService, ProductIntegrationService>();
            services.AddScoped<ISpecificationAttributeOptionService, SpecificationAttributeOptionService>();
            services.AddScoped<ISpecificationAttributeService, SpecificationAttributeService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IImportProfileService, ImportProfileService>();
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<IShipmentItemService, ShipmentItemService>();
            services.AddScoped<IProductSpecificationAttributeMappingService, ProductSpecificationAttributeMappingService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICustomerAddressMappingService, CustomerAddressMappingService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<ITaskExecutionInfoService, TaskExecutionInfoService>();
            services.AddScoped<ITaskExecutionInfoRepository, TaskExecutionInfoRepository>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IEventPublisher, EventBus>();
            services.AddScoped<ITaskDescriptorService, TaskDescriptorService>();
            services.AddScoped<IReturnRequestService, ReturnRequestService>();
            services.AddScoped<ICrossSellProductService, CrossSellProductService>();
            services.AddScoped<IRelatedProductService, RelatedProductService>();
            services.AddScoped<IArasCargoService, ArasCargoService>();
            services.AddHttpClient();

            return services;
        }
    }
}
