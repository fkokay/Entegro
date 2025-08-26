using AutoMapper;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services
{
    public class ProductVariantAttributeValueService : IProductVariantAttributeValueService
    {
        private readonly IProductVariantAttributeValueRepository _productVariantAttributeValueRepository;
        private readonly IMapper _mapper;

        public ProductVariantAttributeValueService(IProductVariantAttributeValueRepository productVariantAttributeValueRepository, IMapper mapper)
        {
            _productVariantAttributeValueRepository = productVariantAttributeValueRepository ?? throw new ArgumentNullException(nameof(productVariantAttributeValueRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(ProductVariantAttributeValueDto data)
        {
            var model = _mapper.Map<ProductVariantAttributeValue>(data);
            await _productVariantAttributeValueRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<ProductVariantAttributeValueDto?> GetByNameAsync(string name)
        {
            var productAttributeValue = await _productVariantAttributeValueRepository.GetByNameAsync(name);
            if (productAttributeValue == null)
            {
                return null;
            }

            var productAttributeValueDto = _mapper.Map<ProductVariantAttributeValueDto>(productAttributeValue);
            return productAttributeValueDto;
        }
    }
}
