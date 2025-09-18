using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.City;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Country;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.District;
using Entegro.Application.DTOs.EmailAccount;
using Entegro.Application.DTOs.ImportProfile;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemLog;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.DTOs.MediaFolder;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.OrderNote;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductSpecificationAttribute;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.Setting;
using Entegro.Application.DTOs.Shipment;
using Entegro.Application.DTOs.ShipmentItem;
using Entegro.Application.DTOs.SpecificationAttribute;
using Entegro.Application.DTOs.SpecificationAttributeOption;
using Entegro.Application.DTOs.Town;
using Entegro.Application.DTOs.User;
using Entegro.Domain.Entities.Catalog;
using Entegro.Domain.Entities.Checkout;
using Entegro.Domain.Entities.Common;
using Entegro.Domain.Entities.Content;
using Entegro.Domain.Entities.Import;
using Entegro.Domain.Entities.Integration;
using Entegro.Domain.Entities.Platform.Identity;
using Entegro.Domain.Entities.Platform.Messaging;
using Entegro.Domain.Entities.Setttings;
using Mapster;
using Order = Entegro.Domain.Entities.Checkout.Order;

namespace Entegro.Application.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            var config = TypeAdapterConfig.GlobalSettings;

            // ---------------- Generic PagedResult mapping ----------------
            TypeAdapterConfig.GlobalSettings.ForType(typeof(PagedResult<>), typeof(PagedResult<>));

            // ---------------- Product ----------------
            config.NewConfig<Product, ProductDto>().TwoWays();
            config.NewConfig<Product, CreateProductDto>().TwoWays();
            config.NewConfig<Product, UpdateProductDto>().TwoWays();

            // ---------------- ProductMediaFile ----------------
            config.NewConfig<ProductMediaFile, ProductMediaFileDto>().TwoWays();
            config.NewConfig<ProductMediaFile, CreateProductMediaFileDto>().TwoWays();
            config.NewConfig<ProductMediaFile, UpdateProductMediaFileDto>().TwoWays();

            // ---------------- ProductCategory ----------------
            config.NewConfig<ProductCategory, ProductCategoryDto>().TwoWays();
            config.NewConfig<ProductCategory, CreateProductCategoryDto>().TwoWays();
            config.NewConfig<ProductCategory, UpdateProductCategoryDto>().TwoWays();


            // ----------------  ProductSpecificationAttribute ----------------
            config.NewConfig<ProductSpecificationAttribute, ProductSpecificationAttributeDto>().TwoWays();
            config.NewConfig<ProductSpecificationAttribute, CreateProductSpecificationAttributeDto>().TwoWays();
            config.NewConfig<ProductSpecificationAttribute, UpdateProductSpecificationAttributeDto>().TwoWays();

            // ---------------- Brand ----------------
            config.NewConfig<Brand, BrandDto>().TwoWays();
            config.NewConfig<Brand, CreateBrandDto>().TwoWays();
            config.NewConfig<Brand, UpdateBrandDto>().TwoWays();

            // ---------------- Category ----------------
            config.NewConfig<Category, CategoryDto>().TwoWays();
            config.NewConfig<Category, CreateCategoryDto>().TwoWays();
            config.NewConfig<Category, UpdateCategoryDto>().TwoWays();

            // ---------------- Order & OrderItem ----------------
            config.NewConfig<Order, OrderDto>().TwoWays();
            config.NewConfig<Order, CreateOrderDto>().TwoWays();
            config.NewConfig<Order, UpdateOrderDto>().TwoWays();
            config.NewConfig<OrderItem, OrderItemDto>().TwoWays();
            config.NewConfig<OrderItem, CreateOrderItemDto>().TwoWays();
            config.NewConfig<OrderItem, UpdateOrderItemDto>().TwoWays();
            config.NewConfig<OrderNote, OrderNoteDto>().TwoWays();

            // ---------------- Customer ----------------
            config.NewConfig<Customer, CustomerDto>().TwoWays();
            config.NewConfig<Customer, CreateCustomerDto>().TwoWays();
            config.NewConfig<Customer, UpdateCustomerDto>().TwoWays();

            // ---------------- User ----------------
            config.NewConfig<User, UserDto>().TwoWays();
            config.NewConfig<User, CreateUserDto>().TwoWays();
            config.NewConfig<User, UpdateUserDto>().TwoWays();

            // ---------------- MediaFolder ----------------
            config.NewConfig<MediaFolder, MediaFolderDto>().TwoWays();
            config.NewConfig<MediaFolder, CreateMediaFolderDto>().TwoWays();
            config.NewConfig<MediaFolder, UpdateMediaFolderDto>().TwoWays();

            // ---------------- MediaFile ----------------
            config.NewConfig<MediaFile, MediaFileDto>().TwoWays();
            config.NewConfig<MediaFile, CreateMediaFileDto>().TwoWays();
            config.NewConfig<MediaFile, UpdateMediaFileDto>().TwoWays();

            // ---------------- ProductAttribute & Value ----------------
            config.NewConfig<ProductAttribute, ProductAttributeDto>().TwoWays();
            config.NewConfig<ProductAttributeValue, ProductAttributeValueDto>().TwoWays();
            config.NewConfig<ProductVariantAttribute, ProductVariantAttributeDto>().TwoWays();
            config.NewConfig<ProductVariantAttributeCombination, ProductVariantAttributeCombinationDto>().TwoWays();

            // ---------------- IntegrationSystem ----------------
            config.NewConfig<IntegrationSystem, IntegrationSystemDto>().TwoWays();
            config.NewConfig<IntegrationSystem, CreateIntegrationSystemDto>().TwoWays();
            config.NewConfig<IntegrationSystem, UpdateIntegrationSystemDto>().TwoWays();

            config.NewConfig<IntegrationSystemParameter, IntegrationSystemParameterDto>().TwoWays();
            config.NewConfig<IntegrationSystemLog, IntegrationSystemLogDto>().TwoWays();

            // ---------------- Location ----------------
            config.NewConfig<City, CityDto>().TwoWays();
            config.NewConfig<Town, TownDto>().TwoWays();
            config.NewConfig<District, DistrictDto>().TwoWays();
            config.NewConfig<Country, CountryDto>().TwoWays();

            // ---------------- ProductIntegration ----------------
            config.NewConfig<ProductIntegration, ProductIntegrationDto>().TwoWays();

            // ---------------- SpecificationAttribute ----------------
            config.NewConfig<SpecificationAttribute, SpecificationAttributeDto>().TwoWays();
            config.NewConfig<SpecificationAttributeOption, SpecificationAttributeOptionDto>().TwoWays();

            // ---------------- Address & Email ----------------
            config.NewConfig<Address, AddressDto>().TwoWays();
            config.NewConfig<EmailAccount, EmailAccountDto>().TwoWays();


            // ---------------- Order ----------------
            config.NewConfig<Order, OrderDto>().TwoWays();
            config.NewConfig<Order, CreateOrderDto>().TwoWays();
            config.NewConfig<Order, UpdateOrderDto>().TwoWays();


            // ---------------- ImportProfile ----------------
            config.NewConfig<ImportProfile, ImportProfileDto>().TwoWays();
            config.NewConfig<ImportProfile, CreateImportProfileDto>().TwoWays();
            config.NewConfig<ImportProfile, UpdateImportProfileDto>().TwoWays();


            // ---------------- Shipment ----------------
            config.NewConfig<Shipment, ShipmentDto>().TwoWays();
            config.NewConfig<Shipment, CreateShipmentDto>().TwoWays();
            config.NewConfig<Shipment, UpdateShipmentDto>().TwoWays();


            // ---------------- ShipmentItem ----------------
            config.NewConfig<ShipmentItem, ShipmentItemDto>().TwoWays();
            config.NewConfig<ShipmentItem, CreateShipmentItemDto>().TwoWays();
            config.NewConfig<ShipmentItem, UpdateShipmentItemDto>().TwoWays();


            // ---------------- Setting ----------------
            config.NewConfig<Setting, SettingDto>().TwoWays();
            config.NewConfig<Setting, CreateSettingDto>().TwoWays();
            config.NewConfig<Setting, UpdateSettingDto>().TwoWays();


        }
    }
}
