using AutoMapper;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.Smartstore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings
{
    public class SmartstoreMappingProfile : Profile
    {
        public SmartstoreMappingProfile()
        {
            CreateMap<SmartstoreProductDto, CreateProductDto>()
                .ForMember(m => m.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(m => m.Description, opt => opt.MapFrom(src => src.FullDescription))
                .ForMember(m => m.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(m => m.Code, opt => opt.MapFrom(src => src.Sku));

            CreateMap<SmartstoreOrderDto, CreateOrderDto>()
                .ForMember(m => m.OrderNo, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(m => m.OrderDate, opt => opt.MapFrom(src => src.CreatedOnUtc));
        }
    }
}
