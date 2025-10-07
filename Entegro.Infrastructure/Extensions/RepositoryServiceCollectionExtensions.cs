using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Entegro.Infrastructure.Extensions
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IMediaFolderRepository, MediaFolderRepository>();
            services.AddScoped<IMediaFileRepository, MediaFileRepository>();
            services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
            services.AddScoped<IProductAttributeValueRepository, ProductAttributeValueRepository>();
            services.AddScoped<IProductVariantAttributeRepository, ProductVariantAttributeRepository>();
            services.AddScoped<IProductVariantAttributeValueRepository, ProductVariantAttributeValueRepository>();
            services.AddScoped<IProductVariantAttributeCombinationRepository, ProductVariantAttributeCombinationRepository>();
            services.AddScoped<IProductImageMappingRepository, ProductImageMappingRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IIntegrationSystemRepository, IntegrationSystemRepository>();
            services.AddScoped<IIntegrationSystemParameterRepository, IntegrationSystemParameterRepository>();
            services.AddScoped<IIntegrationSystemLogRepository, IntegrationSystemLogRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ITownRepository, TownRepository>();
            services.AddScoped<IOrderNoteRepository, OrderNoteRepository>();
            services.AddScoped<IProductIntegrationRepository, ProductIntegrationRepository>();
            services.AddScoped<ISpecificationAttributeOptionRepository, SpecificationAttributeOptionRepository>();
            services.AddScoped<ISpecificationAttributeRepository, SpecificationAttributeRepository>();
            services.AddScoped<IEmailAccountRepository, EmailAccountRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IShipmentRepository, ShipmentRepository>();
            services.AddScoped<IShipmentItemRepository, ShipmentItemRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IImportProfileRepository, ImportProfileRepository>();
            services.AddScoped<ITreeNodeRepository<Category>, TreeNodeRepository<Category>>();
            services.AddScoped<IProductSpecificationAttributeMappingRepository, ProductSpecificationAttributeMappingRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IDistrictRepository, DistrictRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ICustomerAddressMappingRepository, CustomerAddressMappingRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<ITaskDescriptorRepository, TaskDescriptorRepository>();
            services.AddScoped<IReturnRequestRepository, ReturnRequestRepository>();
            services.AddScoped<ICrossSellProductRepository, CrossSellProductRepository>();
            services.AddScoped<IRelatedProductRepository, RelatedProductRepository>();
            return services;
        }
    }
}
