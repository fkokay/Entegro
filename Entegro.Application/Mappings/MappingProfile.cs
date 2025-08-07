using AutoMapper;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
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
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<PagedResult<Product>, PagedResult<ProductDto>>().ReverseMap();

            CreateMap<Brand, BrandDto>().ReverseMap();
            CreateMap<PagedResult<Brand>, PagedResult<BrandDto>>().ReverseMap();

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<PagedResult<Category>, PagedResult<CategoryDto>>().ReverseMap();
        }
    }
}
