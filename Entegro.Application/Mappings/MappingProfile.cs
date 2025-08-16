using AutoMapper;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.DTOs.ProductImage;
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

            #region user
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
        }
    }
}
