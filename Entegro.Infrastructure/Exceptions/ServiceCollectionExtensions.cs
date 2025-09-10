using Entegro.Application.Interfaces.Services;
using Entegro.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Exceptions
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
            services.AddScoped<IProductImageMappingService, ProductImageMappingService>();
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
            return services;
        }
    }
}
