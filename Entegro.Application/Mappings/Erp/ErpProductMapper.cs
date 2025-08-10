using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductImage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Erp
{
    public static class ErpProductMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static ProductDto? ToDto(ErpProductDto erpProduct)
        {
            if (erpProduct == null)
            {
                return null;
            }

            ProductDto product = new ProductDto();
            product.Name = erpProduct.Name;
            product.Code = erpProduct.Code;

            return product; ;
        }

        public static IEnumerable<ProductDto> ToDtoList(IEnumerable<ErpProductDto> products)
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
