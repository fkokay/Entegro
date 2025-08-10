using AutoMapper;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Product;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            CreateMap<PagedResult<Order>, PagedResult<OrderDto>>().ReverseMap();
            #endregion

            #region OrderItem
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<OrderItem, CreateOrderItemDto>().ReverseMap();
            CreateMap<OrderItem, UpdateOrderItemDto>().ReverseMap();
            CreateMap<PagedResult<OrderItem>, PagedResult<OrderItemDto>>().ReverseMap();
            #endregion

            #region Customer
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CreateCustomerDto>().ReverseMap();
            CreateMap<Customer, UpdateCustomerDto>().ReverseMap();
            CreateMap<PagedResult<Customer>, PagedResult<CustomerDto>>().ReverseMap(); 
            #endregion
        }
    }
}
