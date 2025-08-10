using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Entegro.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreProductMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }
        public static ProductDto? ToDto(SmartstoreProductDto smartstoreProduct)
        {
            try
            {
                if (smartstoreProduct == null)
                {
                    return null;
                }

                ProductDto productDto = new ProductDto();
                productDto.Name = smartstoreProduct.Name;
                productDto.Code = smartstoreProduct.Sku;
                productDto.Description = smartstoreProduct.FullDescription;
                productDto.Price = smartstoreProduct.Price;
                return productDto; ;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Product mapping sırasında hata oluştu. ProductId: {ProductId}", smartstoreProduct.Id);
                return null;
            }
        }

        public static IEnumerable<ProductDto> ToDtoList(IEnumerable<SmartstoreProductDto> products)
        {
            if (products == null)
                yield break;

            foreach (var product in products)
            {
                var dto = ToDto(product);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
