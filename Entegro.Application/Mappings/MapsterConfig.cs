using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.City;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Country;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.District;
using Entegro.Application.DTOs.EmailAccount;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemLog;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.DTOs.MediaFolder;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.DTOs.SpecificationAttribute;
using Entegro.Application.DTOs.SpecificationAttributeOption;
using Entegro.Application.DTOs.Town;
using Entegro.Application.DTOs.User;
using Entegro.Domain.Entities;
using Mapster;

namespace Entegro.Application.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            var config = TypeAdapterConfig.GlobalSettings;

            // ---------------- Product ----------------
            config.NewConfig<Product, ProductDto>().TwoWays();
            config.NewConfig<Product, CreateProductDto>().TwoWays();
            config.NewConfig<Product, UpdateProductDto>().TwoWays();

            config.NewConfig<ProductDto, CreateProductDto>().TwoWays();
            config.NewConfig<ProductDto, UpdateProductDto>().TwoWays();

            config.NewConfig<CreateProductDto, ProductDto>().TwoWays();
            config.NewConfig<UpdateProductDto, ProductDto>().TwoWays();

            config.NewConfig<PagedResult<Product>, PagedResult<ProductDto>>().TwoWays();
            config.NewConfig<PagedResult<ProductDto>, PagedResult<Product>>().TwoWays();

            // ---------------- ProductMediaFile ----------------
            config.NewConfig<ProductMediaFile, ProductMediaFileDto>().TwoWays();
            config.NewConfig<ProductMediaFile, CreateProductMediaFileDto>().TwoWays();
            config.NewConfig<ProductMediaFile, UpdateProductMediaFileeDto>().TwoWays();

            config.NewConfig<ProductMediaFileDto, CreateProductMediaFileDto>().TwoWays();
            config.NewConfig<ProductMediaFileDto, UpdateProductMediaFileeDto>().TwoWays();

            config.NewConfig<CreateProductMediaFileDto, ProductMediaFileDto>().TwoWays();
            config.NewConfig<UpdateProductMediaFileeDto, ProductMediaFileDto>().TwoWays();

            config.NewConfig<PagedResult<ProductMediaFile>, PagedResult<ProductMediaFileDto>>().TwoWays();
            config.NewConfig<PagedResult<ProductMediaFileDto>, PagedResult<ProductMediaFile>>().TwoWays();

            // ---------------- ProductCategory ----------------
            config.NewConfig<ProductCategory, ProductCategoryDto>().TwoWays();
            config.NewConfig<ProductCategory, CreateProductCategoryDto>().TwoWays();
            config.NewConfig<ProductCategory, UpdateProductCategoryDto>().TwoWays();

            // ---------------- Brand ----------------
            config.NewConfig<Brand, BrandDto>().TwoWays();
            config.NewConfig<Brand, CreateBrandDto>().TwoWays();
            config.NewConfig<Brand, UpdateBrandDto>().TwoWays();

            config.NewConfig<BrandDto, CreateBrandDto>().TwoWays();
            config.NewConfig<BrandDto, UpdateBrandDto>().TwoWays();

            config.NewConfig<CreateBrandDto, BrandDto>().TwoWays();
            config.NewConfig<UpdateBrandDto, BrandDto>().TwoWays();

            config.NewConfig<PagedResult<Brand>, PagedResult<BrandDto>>().TwoWays();
            config.NewConfig<PagedResult<BrandDto>, PagedResult<Brand>>().TwoWays();

            // ---------------- Category ----------------
            config.NewConfig<Category, CategoryDto>().TwoWays();
            config.NewConfig<Category, CreateCategoryDto>().TwoWays();
            config.NewConfig<Category, UpdateCategoryDto>().TwoWays();

            config.NewConfig<CategoryDto, CreateCategoryDto>().TwoWays();
            config.NewConfig<CategoryDto, UpdateCategoryDto>().TwoWays();

            config.NewConfig<CreateCategoryDto, CategoryDto>().TwoWays();
            config.NewConfig<UpdateCategoryDto, CategoryDto>().TwoWays();

            config.NewConfig<PagedResult<Category>, PagedResult<CategoryDto>>().TwoWays();
            config.NewConfig<PagedResult<CategoryDto>, PagedResult<Category>>().TwoWays();

            // ---------------- Order ----------------
            config.NewConfig<Order, OrderDto>().TwoWays();
            config.NewConfig<Order, CreateOrderDto>().TwoWays();
            config.NewConfig<Order, UpdateOrderDto>().TwoWays();

            config.NewConfig<OrderDto, CreateOrderDto>().TwoWays();
            config.NewConfig<OrderDto, UpdateOrderDto>().TwoWays();

            config.NewConfig<CreateOrderDto, OrderDto>().TwoWays();
            config.NewConfig<UpdateOrderDto, OrderDto>().TwoWays();

            config.NewConfig<PagedResult<Order>, PagedResult<OrderDto>>().TwoWays();
            config.NewConfig<PagedResult<ProductDto>, PagedResult<Product>>().TwoWays();

            // ---------------- OrderItem ----------------
            config.NewConfig<OrderItem, OrderItemDto>().TwoWays();
            config.NewConfig<OrderItem, CreateOrderItemDto>().TwoWays();
            config.NewConfig<OrderItem, UpdateOrderItemDto>().TwoWays();

            config.NewConfig<OrderItemDto, CreateOrderItemDto>().TwoWays();
            config.NewConfig<OrderItemDto, UpdateOrderItemDto>().TwoWays();

            config.NewConfig<CreateOrderItemDto, OrderItemDto>().TwoWays();
            config.NewConfig<UpdateOrderItemDto, OrderItemDto>().TwoWays();

            config.NewConfig<PagedResult<OrderItem>, PagedResult<OrderItemDto>>().TwoWays();
            config.NewConfig<PagedResult<OrderItemDto>, PagedResult<Product>>().TwoWays();

            // ---------------- Customer ----------------
            config.NewConfig<Customer, CustomerDto>().TwoWays();
            config.NewConfig<Customer, CreateCustomerDto>().TwoWays();
            config.NewConfig<Customer, UpdateCustomerDto>().TwoWays();

            config.NewConfig<PagedResult<Customer>, PagedResult<CustomerDto>>().TwoWays();

            // ---------------- User ----------------
            config.NewConfig<User, UserDto>().TwoWays();
            config.NewConfig<User, CreateUserDto>().TwoWays();
            config.NewConfig<User, UpdateUserDto>().TwoWays();

            config.NewConfig<UserDto, CreateUserDto>().TwoWays();
            config.NewConfig<UserDto, UpdateUserDto>().TwoWays();

            config.NewConfig<CreateUserDto, UserDto>().TwoWays();
            config.NewConfig<UpdateUserDto, UserDto>().TwoWays();

            config.NewConfig<PagedResult<User>, PagedResult<UserDto>>().TwoWays();
            config.NewConfig<PagedResult<UserDto>, PagedResult<User>>().TwoWays();

            // ---------------- MediaFolder ----------------
            config.NewConfig<MediaFolder, MediaFolderDto>().TwoWays();
            config.NewConfig<MediaFolder, CreateMediaFolderDto>().TwoWays();
            config.NewConfig<MediaFolder, UpdateMediaFolderDto>().TwoWays();
            config.NewConfig<PagedResult<MediaFolder>, PagedResult<MediaFolderDto>>().TwoWays();

            // ---------------- MediaFile ----------------
            config.NewConfig<MediaFile, MediaFileDto>().TwoWays();
            config.NewConfig<MediaFile, CreateMediaFileDto>().TwoWays();
            config.NewConfig<MediaFile, UpdateMediaFileDto>().TwoWays();
            config.NewConfig<PagedResult<MediaFile>, PagedResult<MediaFileDto>>().TwoWays();

            // ---------------- ProductAttribute ----------------
            config.NewConfig<ProductAttribute, ProductAttributeDto>().TwoWays();
            config.NewConfig<ProductAttribute, CreateProductAttributeDto>().TwoWays();
            config.NewConfig<ProductAttribute, UpdateProductAttributeDto>().TwoWays();
            config.NewConfig<PagedResult<ProductAttribute>, PagedResult<ProductAttributeDto>>().TwoWays();

            // ---------------- ProductAttributeValue ----------------
            config.NewConfig<ProductAttributeValue, ProductAttributeValueDto>().TwoWays();
            config.NewConfig<ProductAttributeValue, CreateProductAttributeValueDto>().TwoWays();
            config.NewConfig<ProductAttributeValue, UpdateProductAttributeValueDto>().TwoWays();
            config.NewConfig<PagedResult<ProductAttributeValue>, PagedResult<ProductAttributeValueDto>>().TwoWays();

            // ---------------- ProductVariantAttribute ----------------
            config.NewConfig<ProductVariantAttribute, ProductVariantAttributeDto>().TwoWays();
            config.NewConfig<ProductVariantAttribute, CreateProductVariantAttributeDto>().TwoWays();
            config.NewConfig<ProductVariantAttribute, UpdateProductVariantAttributeDto>().TwoWays();

            // ---------------- ProductVariantAttributeCombination ----------------
            config.NewConfig<ProductVariantAttributeCombination, ProductVariantAttributeCombinationDto>().TwoWays();
            config.NewConfig<ProductVariantAttributeCombination, CreateProductVariantAttributeCombinationDto>().TwoWays();
            config.NewConfig<ProductVariantAttributeCombination, UpdateProductVariantAttributeCombinationDto>().TwoWays();

            // ---------------- IntegrationSystemLog ----------------
            config.NewConfig<IntegrationSystemLog, IntegrationSystemLogDto>().TwoWays();
            config.NewConfig<IntegrationSystemLog, CreateIntegrationSystemLogDto>().TwoWays();
            config.NewConfig<IntegrationSystemLog, UpdateIntegrationSystemLogDto>().TwoWays();

            // ---------------- IntegrationSystemParameter ----------------
            config.NewConfig<IntegrationSystemParameter, IntegrationSystemParameterDto>().TwoWays();
            config.NewConfig<IntegrationSystemParameter, CreateIntegrationSystemParameterDto>().TwoWays();
            config.NewConfig<IntegrationSystemParameter, UpdateIntegrationSystemParameterDto>().TwoWays();

            config.NewConfig<IntegrationSystemParameterDto, CreateIntegrationSystemParameterDto>().TwoWays();
            config.NewConfig<IntegrationSystemParameterDto, UpdateIntegrationSystemParameterDto>().TwoWays();

            config.NewConfig<CreateIntegrationSystemParameterDto, IntegrationSystemParameterDto>().TwoWays();
            config.NewConfig<UpdateIntegrationSystemParameterDto, IntegrationSystemParameterDto>().TwoWays();

            config.NewConfig<PagedResult<IntegrationSystemParameter>, PagedResult<IntegrationSystemParameterDto>>().TwoWays();
            config.NewConfig<PagedResult<IntegrationSystemParameterDto>, PagedResult<IntegrationSystemParameter>>().TwoWays();

            // ---------------- IntegrationSystem ----------------
            config.NewConfig<IntegrationSystem, IntegrationSystemDto>().TwoWays();
            config.NewConfig<IntegrationSystem, CreateIntegrationSystemDto>().TwoWays();
            config.NewConfig<IntegrationSystem, UpdateIntegrationSystemDto>().TwoWays();

            // ---------------- City ----------------
            config.NewConfig<City, CityDto>().TwoWays();
            config.NewConfig<CreateCityDto, City>().TwoWays();
            config.NewConfig<UpdateCityDto, City>().TwoWays();
            config.NewConfig<PagedResult<City>, PagedResult<CityDto>>().TwoWays();

            // ---------------- Town ----------------
            config.NewConfig<Town, TownDto>().TwoWays();
            config.NewConfig<CreateTownDto, Town>().TwoWays();
            config.NewConfig<UpdateTownDto, Town>().TwoWays();
            config.NewConfig<PagedResult<Town>, PagedResult<TownDto>>().TwoWays();

            // ---------------- District ----------------
            config.NewConfig<District, DistrictDto>().TwoWays();
            config.NewConfig<CreateDistrictDto, District>().TwoWays();
            config.NewConfig<UpdateDistrictDto, District>().TwoWays();

            // ---------------- Country ----------------
            config.NewConfig<Country, CountryDto>().TwoWays();
            config.NewConfig<Country, CreateCountryDto>().TwoWays();
            config.NewConfig<Country, UpdateCountryDto>().TwoWays();
            config.NewConfig<PagedResult<Country>, PagedResult<CountryDto>>().TwoWays();

            // ---------------- ProductIntegration ----------------
            config.NewConfig<ProductIntegration, ProductIntegrationDto>().TwoWays();
            config.NewConfig<ProductIntegration, CreateProductIntegrationDto>().TwoWays();
            config.NewConfig<ProductIntegration, UpdateProductIntegrationDto>().TwoWays();

            config.NewConfig<ProductIntegrationDto, CreateProductIntegrationDto>().TwoWays();
            config.NewConfig<ProductIntegrationDto, UpdateProductIntegrationDto>().TwoWays();

            config.NewConfig<CreateProductIntegrationDto, ProductIntegrationDto>().TwoWays();
            config.NewConfig<UpdateProductIntegrationDto, ProductIntegrationDto>().TwoWays();

            config.NewConfig<PagedResult<ProductIntegration>, PagedResult<ProductIntegrationDto>>().TwoWays();
            config.NewConfig<PagedResult<ProductIntegrationDto>, PagedResult<ProductIntegration>>().TwoWays();

            // ---------------- ProductVariantAttributeValue ----------------
            config.NewConfig<ProductVariantAttributeValue, ProductVariantAttributeValueDto>().TwoWays();
            config.NewConfig<PagedResult<Product>, PagedResult<ProductVariantAttributeValueDto>>().TwoWays();
            config.NewConfig<PagedResult<ProductVariantAttributeValueDto>, PagedResult<Product>>().TwoWays();

            // ---------------- SpecificationAttribute ----------------
            config.NewConfig<SpecificationAttribute, SpecificationAttributeDto>().TwoWays();
            config.NewConfig<SpecificationAttribute, CreateSpecificationAttributeDto>().TwoWays();
            config.NewConfig<SpecificationAttribute, UpdateSpecificationAttributeDto>().TwoWays();
            config.NewConfig<PagedResult<SpecificationAttribute>, PagedResult<SpecificationAttributeDto>>().TwoWays();

            // ---------------- SpecificationAttributeOption ----------------
            config.NewConfig<SpecificationAttributeOption, SpecificationAttributeOptionDto>().TwoWays();
            config.NewConfig<SpecificationAttributeOption, CreateSpecificationAttributeOptionDto>().TwoWays();
            config.NewConfig<SpecificationAttributeOption, UpdateSpecificationAttributeOptionDto>().TwoWays();
            config.NewConfig<PagedResult<SpecificationAttributeOption>, PagedResult<SpecificationAttributeOptionDto>>().TwoWays();

            // ---------------- Address ----------------
            config.NewConfig<Address, AddressDto>().TwoWays();
            config.NewConfig<Address, CreateAddressDto>().TwoWays();
            config.NewConfig<Address, UpdateAddressDto>().TwoWays();
            config.NewConfig<PagedResult<Address>, PagedResult<AddressDto>>().TwoWays();

            // ---------------- EmailAccount ----------------
            config.NewConfig<EmailAccount, EmailAccountDto>().TwoWays();
            config.NewConfig<EmailAccount, CreateEmailAccountDto>().TwoWays();
            config.NewConfig<EmailAccount, UpdateEmailAccountDto>().TwoWays();
            config.NewConfig<PagedResult<EmailAccount>, PagedResult<EmailAccountDto>>().TwoWays();
        }
    }
}
