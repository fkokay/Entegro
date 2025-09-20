using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.EmailAccount;
using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.OrderNote;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Web.Models;
using Entegro.Web.Models.Catalog.Attributes;
using Entegro.Web.Models.Catalog.Brands;
using Entegro.Web.Models.Catalog.Categories;
using Entegro.Web.Models.Catalog.Products;
using Entegro.Web.Models.Checkout.Orders;
using Entegro.Web.Models.Common;
using Entegro.Web.Models.Content;
using Entegro.Web.Models.Platform.Messaging;
using Mapster;

namespace Entegro.Web.Mappings
{
    public static class WebMapsterConfig
    {
        public static void RegisterMappings()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            TypeAdapterConfig.GlobalSettings.ForType(typeof(PagedResult<>), typeof(PagedResult<>));

            config.NewConfig<ProductModel, ProductDto>().TwoWays();
            config.NewConfig<ProductModel, CreateProductDto>().TwoWays();
            config.NewConfig<ProductModel, UpdateProductDto>().TwoWays();

            config.NewConfig<ProductMediaFileModel, ProductMediaFileModel>().TwoWays();
            config.NewConfig<ProductMediaFileModel, CreateProductMediaFileDto>().TwoWays();
            config.NewConfig<ProductMediaFileModel, UpdateProductMediaFileDto>().TwoWays();

            config.NewConfig<ProductCategoryModel, ProductCategoryDto>().TwoWays();
            config.NewConfig<ProductCategoryModel, CreateProductCategoryDto>().TwoWays();
            config.NewConfig<ProductCategoryModel, UpdateProductCategoryDto>().TwoWays();

            config.NewConfig<ProductVariantAttributeModel, ProductVariantAttributeDto>().TwoWays();
            config.NewConfig<ProductVariantAttributeModel, CreateProductVariantAttributeDto>().TwoWays();
            config.NewConfig<ProductVariantAttributeModel, UpdateProductVariantAttributeDto>().TwoWays();

            config.NewConfig<ProductVariantAttributeValueModel, ProductVariantAttributeValueDto>().TwoWays();
            config.NewConfig<ProductVariantAttributeValueModel, CreateProductVariantAttributeValueDto>().TwoWays();

            config.NewConfig<ProductVariantAttributeCombinationModel, ProductVariantAttributeCombinationDto>().TwoWays();
            config.NewConfig<ProductVariantAttributeCombinationModel, CreateProductVariantAttributeCombinationDto>().TwoWays();
            config.NewConfig<ProductVariantAttributeCombinationModel, UpdateProductVariantAttributeCombinationDto>().TwoWays();

            config.NewConfig<ProductAttributeModel, ProductAttributeDto>().TwoWays();
            config.NewConfig<ProductAttributeModel, CreateProductAttributeDto>().TwoWays();
            config.NewConfig<ProductAttributeModel, UpdateProductAttributeDto>().TwoWays();

            config.NewConfig<ProductAttributeValueModel, ProductAttributeValueDto>().TwoWays();
            config.NewConfig<ProductAttributeValueModel, CreateProductAttributeValueDto>().TwoWays();
            config.NewConfig<ProductAttributeValueModel, UpdateProductAttributeValueDto>().TwoWays();

            config.NewConfig<BrandModel, BrandDto>().TwoWays();
            config.NewConfig<BrandModel, CreateBrandDto>().TwoWays();
            config.NewConfig<BrandModel, UpdateBrandDto>().TwoWays();

            config.NewConfig<CategoryModel, CategoryDto>().TwoWays();
            config.NewConfig<CategoryModel, CreateCategoryDto>().TwoWays();
            config.NewConfig<CategoryModel, UpdateCategoryDto>().TwoWays();

            config.NewConfig<CustomerModel, CustomerDto>().TwoWays();
            config.NewConfig<CustomerModel, CreateCustomerDto>().TwoWays();
            config.NewConfig<CustomerModel, UpdateCustomerDto>().TwoWays();

            config.NewConfig<EmailAccountModel, EmailAccountDto>().TwoWays();
            config.NewConfig<EmailAccountModel, CreateEmailAccountDto>().TwoWays();
            config.NewConfig<EmailAccountModel, UpdateEmailAccountDto>().TwoWays();

            config.NewConfig<OrderModel, OrderDto>().TwoWays();
            config.NewConfig<OrderModel, CreateOrderDto>().TwoWays();
            config.NewConfig<OrderModel, UpdateOrderDto>().TwoWays();

            config.NewConfig<OrderItemModel, OrderItemDto>().TwoWays();
            config.NewConfig<OrderItemModel, CreateOrderItemDto>().TwoWays();
            config.NewConfig<OrderItemModel, UpdateOrderItemDto>().TwoWays();

            config.NewConfig<OrderNoteModel, OrderNoteModel>().TwoWays();
            config.NewConfig<OrderNoteModel, CreateOrderNoteDto>().TwoWays();
            config.NewConfig<OrderNoteModel, UpdateOrderNoteDto>().TwoWays();

            config.NewConfig<AddressModel, AddressDto>().TwoWays();
            config.NewConfig<AddressModel, CreateAddressDto>().TwoWays();
            config.NewConfig<AddressModel, UpdateAddressDto>().TwoWays();

            config.NewConfig<MediaFileModel, MediaFileDto>().TwoWays();
            config.NewConfig<MediaFolderModel, MediaFolderModel>().TwoWays();
        }
    }
}
