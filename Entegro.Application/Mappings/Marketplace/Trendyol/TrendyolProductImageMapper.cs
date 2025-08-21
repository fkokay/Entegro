using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductImage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.Trendyol
{
    public static class TrendyolProductImageMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static ProductImageDto? ToDto(TrendyolProductImageDto trendyolProductImage)
        {
            if (trendyolProductImage == null)
            {
                return null;
            }

            ProductImageDto productImage = new ProductImageDto();
            productImage.DisplayOrder = 0;

            return productImage; ;
        }

        public static IEnumerable<ProductImageDto> ToDtoList(IEnumerable<TrendyolProductImageDto> productImages)
        {
            if (productImages == null)
                yield break;

            foreach (var productImage in productImages)
            {
                var dto = ToDto(productImage);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
