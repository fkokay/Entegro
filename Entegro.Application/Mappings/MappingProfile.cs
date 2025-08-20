using AutoMapper;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.City;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Country;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.District;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemLog;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.DTOs.MediaFolder;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeMapping;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductImage;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.Town;
using Entegro.Application.DTOs.User;
using Entegro.Domain.Entities;

namespace Entegro.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Product
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();

            CreateMap<ProductDto, CreateProductDto>().ReverseMap();
            CreateMap<ProductDto, UpdateProductDto>().ReverseMap();

            CreateMap<CreateProductDto, ProductDto>().ReverseMap();
            CreateMap<UpdateProductDto, ProductDto>().ReverseMap();

            CreateMap<PagedResult<Product>, PagedResult<ProductDto>>().ReverseMap();
            CreateMap<PagedResult<ProductDto>, PagedResult<Product>>().ReverseMap();
            #endregion

            #region ProductImage
            CreateMap<ProductImageMapping, ProductImageDto>().ReverseMap();
            CreateMap<ProductImageMapping, CreateProductImageDto>().ReverseMap();
            CreateMap<ProductImageMapping, UpdateProductImageDto>().ReverseMap();

            CreateMap<ProductImageDto, CreateProductImageDto>().ReverseMap();
            CreateMap<ProductImageDto, UpdateProductImageDto>().ReverseMap();

            CreateMap<CreateProductImageDto, ProductImageDto>().ReverseMap();
            CreateMap<UpdateProductImageDto, ProductImageDto>().ReverseMap();

            CreateMap<PagedResult<ProductImageMapping>, PagedResult<ProductImageDto>>().ReverseMap();
            CreateMap<PagedResult<ProductImageDto>, PagedResult<ProductImageMapping>>().ReverseMap();
            #endregion

            #region ProductCategoryMapping
            CreateMap<ProductCategoryMapping, ProductCategoryDto>().ReverseMap();
            CreateMap<ProductCategoryMapping, CreateProductCategoryDto>().ReverseMap();
            CreateMap<ProductCategoryMapping, UpdateProductCategoryDto>().ReverseMap();
            #endregion

            #region Brand
            CreateMap<Brand, BrandDto>().ReverseMap();
            CreateMap<Brand, CreateBrandDto>().ReverseMap();
            CreateMap<Brand, UpdateBrandDto>().ReverseMap();

            CreateMap<BrandDto, CreateBrandDto>().ReverseMap();
            CreateMap<BrandDto, UpdateBrandDto>().ReverseMap();

            CreateMap<CreateBrandDto, BrandDto>().ReverseMap();
            CreateMap<UpdateBrandDto, BrandDto>().ReverseMap();

            CreateMap<PagedResult<Brand>, PagedResult<BrandDto>>().ReverseMap();
            CreateMap<PagedResult<BrandDto>, PagedResult<Brand>>().ReverseMap();
            #endregion

            #region Category
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryDto>().ReverseMap();
            CreateMap<PagedResult<Category>, PagedResult<CategoryDto>>().ReverseMap();
            #endregion

            #region Order
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Order, CreateOrderDto>().ReverseMap();
            CreateMap<Order, UpdateOrderDto>().ReverseMap();

            CreateMap<OrderDto, CreateOrderDto>().ReverseMap();
            CreateMap<OrderDto, UpdateOrderDto>().ReverseMap();

            CreateMap<CreateOrderDto, OrderDto>().ReverseMap();
            CreateMap<UpdateOrderDto, OrderDto>().ReverseMap();
            CreateMap<PagedResult<Order>, PagedResult<OrderDto>>().ReverseMap();
            CreateMap<PagedResult<ProductDto>, PagedResult<Product>>().ReverseMap();
            #endregion

            #region OrderItem
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<OrderItem, CreateOrderItemDto>().ReverseMap();
            CreateMap<OrderItem, UpdateOrderItemDto>().ReverseMap();

            CreateMap<OrderItemDto, CreateOrderItemDto>().ReverseMap();
            CreateMap<OrderItemDto, UpdateOrderItemDto>().ReverseMap();

            CreateMap<CreateOrderItemDto, OrderItemDto>().ReverseMap();
            CreateMap<UpdateOrderItemDto, OrderItemDto>().ReverseMap();


            CreateMap<PagedResult<OrderItem>, PagedResult<OrderItemDto>>().ReverseMap();
            CreateMap<PagedResult<OrderItemDto>, PagedResult<Product>>().ReverseMap();
            #endregion

            #region Customer
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CreateCustomerDto>().ReverseMap();
            CreateMap<Customer, UpdateCustomerDto>().ReverseMap();
            CreateMap<PagedResult<Customer>, PagedResult<CustomerDto>>().ReverseMap();
            #endregion

            #region User
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<User, UpdateUserDto>().ReverseMap();

            CreateMap<UserDto, CreateUserDto>().ReverseMap();
            CreateMap<UserDto, UpdateUserDto>().ReverseMap();

            CreateMap<CreateUserDto, UserDto>().ReverseMap();
            CreateMap<UpdateUserDto, UserDto>().ReverseMap();

            CreateMap<PagedResult<User>, PagedResult<UserDto>>().ReverseMap();
            CreateMap<PagedResult<UserDto>, PagedResult<User>>().ReverseMap();
            #endregion

            #region mediafolder
            CreateMap<MediaFolder, MediaFolderDto>().ReverseMap();
            CreateMap<MediaFolder, CreateMediaFolderDto>().ReverseMap();
            CreateMap<MediaFolder, UpdateMediaFolderDto>().ReverseMap();
            #endregion

            #region mediafile
            CreateMap<MediaFile, MediaFileDto>().ReverseMap();
            CreateMap<MediaFile, CreateMediaFileDto>().ReverseMap();
            CreateMap<MediaFile, UpdateMediaFileDto>().ReverseMap();
            #endregion

            #region ProductAttribute
            CreateMap<ProductAttribute, ProductAttributeDto>().ReverseMap();
            CreateMap<ProductAttribute, CreateProductAttributeDto>().ReverseMap();
            CreateMap<ProductAttribute, UpdateProductAttributeDto>().ReverseMap();

            CreateMap<ProductAttributeDto, CreateProductAttributeDto>().ReverseMap();
            CreateMap<ProductAttributeDto, UpdateProductAttributeDto>().ReverseMap();

            CreateMap<CreateProductAttributeDto, ProductAttributeDto>().ReverseMap();
            CreateMap<UpdateProductAttributeDto, ProductAttributeDto>().ReverseMap();

            CreateMap<PagedResult<ProductAttribute>, PagedResult<ProductAttributeDto>>().ReverseMap();
            CreateMap<PagedResult<ProductAttributeDto>, PagedResult<ProductAttribute>>().ReverseMap();
            #endregion

            #region ProductAttributeValue
            CreateMap<ProductAttributeValue, ProductAttributeValueDto>().ReverseMap();
            CreateMap<ProductAttributeValue, CreateProductAttributeValueDto>().ReverseMap();
            CreateMap<ProductAttributeValue, UpdateProductAttributeValueDto>().ReverseMap();

            CreateMap<ProductAttributeValueDto, CreateProductAttributeValueDto>().ReverseMap();
            CreateMap<ProductAttributeValueDto, UpdateProductAttributeValueDto>().ReverseMap();

            CreateMap<CreateProductAttributeValueDto, ProductAttributeValueDto>().ReverseMap();
            CreateMap<UpdateProductAttributeValueDto, ProductAttributeValueDto>().ReverseMap();

            CreateMap<PagedResult<ProductAttributeValue>, PagedResult<ProductAttributeValueDto>>().ReverseMap();
            CreateMap<PagedResult<ProductAttributeValueDto>, PagedResult<ProductAttributeValue>>().ReverseMap();
            #endregion

            #region ProductAttributeMapping
            CreateMap<ProductAttributeMapping, ProductAttributeMappingDto>().ReverseMap();
            CreateMap<ProductAttributeMapping, CreateProductAttributeMappingDto>().ReverseMap();
            CreateMap<ProductAttributeMapping, UpdateProductAttributeMappingDto>().ReverseMap();
            #endregion

            #region ProductVariantAttributeCombination
            CreateMap<ProductVariantAttributeCombination, ProductVariantAttributeCombinationDto>().ReverseMap();
            CreateMap<ProductVariantAttributeCombination, CreateProductVariantAttributeCombinationDto>().ReverseMap();
            CreateMap<ProductVariantAttributeCombination, UpdateProductVariantAttributeCombinationDto>().ReverseMap();
            #endregion

            #region IntegrationSystemLog
            CreateMap<IntegrationSystemLog, IntegrationSystemLogDto>().ReverseMap();
            CreateMap<IntegrationSystemLog, CreateIntegrationSystemLogDto>().ReverseMap();
            CreateMap<IntegrationSystemLog, UpdateIntegrationSystemLogDto>().ReverseMap();
            #endregion

            #region IntegrationSystemParameter
            CreateMap<IntegrationSystemParameter, IntegrationSystemParameterDto>().ReverseMap();
            CreateMap<IntegrationSystemParameter, CreateIntegrationSystemParameterDto>().ReverseMap();
            CreateMap<IntegrationSystemParameter, UpdateIntegrationSystemParameterDto>().ReverseMap();
            #endregion

            #region IntegrationSystem
            CreateMap<IntegrationSystem, IntegrationSystemDto>().ReverseMap();
            CreateMap<IntegrationSystem, CreateIntegrationSystemDto>().ReverseMap();
            CreateMap<IntegrationSystem, UpdateIntegrationSystemDto>().ReverseMap();
            #endregion

            #region City
            CreateMap<City, CityDto>().ReverseMap();
            CreateMap<CreateCityDto, City>().ReverseMap();
            CreateMap<UpdateCityDto, City>().ReverseMap();
            CreateMap<PagedResult<City>, PagedResult<CityDto>>().ReverseMap();
            #endregion

            #region Town
            CreateMap<Town, TownDto>().ReverseMap();
            CreateMap<CreateTownDto, Town>().ReverseMap();
            CreateMap<UpdateTownDto, Town>().ReverseMap();
            CreateMap<PagedResult<Town>, PagedResult<TownDto>>().ReverseMap();
            #endregion

            #region District    
            CreateMap<District, DistrictDto>().ReverseMap();
            CreateMap<CreateDistrictDto, District>().ReverseMap();
            CreateMap<UpdateDistrictDto, District>().ReverseMap();
            #endregion
            #region Country    
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, UpdateCountryDto>().ReverseMap();
            CreateMap<PagedResult<Country>, PagedResult<CountryDto>>().ReverseMap();
            #endregion
            #region ProductImage
            CreateMap<ProductImageMapping, ProductImageDto>().ReverseMap();
            CreateMap<ProductImageMapping, CreateProductImageDto>().ReverseMap();
            CreateMap<ProductImageMapping, UpdateProductImageDto>().ReverseMap();
            CreateMap<PagedResult<ProductImageMapping>, PagedResult<ProductImageDto>>().ReverseMap();

            #endregion

        }
    }
}
