using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.Trendyol
{
    public static class TrendyolProductMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }
        public static ProductDto? ToDto(TrendyolProductDto trendyolProduct)
        {
            if (trendyolProduct == null)
            {
                return null;
            }

            TrendyolProductImageMapper.ConfigureLogger(_logger);

            ProductDto productDto = new ProductDto();
            productDto.Name = trendyolProduct.title;
            productDto.Code = trendyolProduct.stockCode;
            productDto.Description = trendyolProduct.description;
            productDto.Price = trendyolProduct.listPrice;
            productDto.MetaKeywords = "";
            productDto.MetaDescription = "";
            productDto.MetaTitle = "";
            productDto.StockQuantity = trendyolProduct.quantity;
            productDto.Barcode = trendyolProduct.barcode;
            productDto.CreatedOn = DateTime.Now;
            productDto.UpdatedOn = DateTime.Now;
            productDto.Brand = new BrandDto()
            {
                Name = trendyolProduct.brand,
                Description = "",
                MetaDescription = "",
                MetaTitle = trendyolProduct.brand,
                MetaKeywords = trendyolProduct.brand.ToLower(),
                DisplayOrder = 0,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
            };
            productDto.ProductImages = TrendyolProductImageMapper.ToDtoList(trendyolProduct.images).ToList();

            return productDto; ;


        }

        public static IEnumerable<ProductDto> ToDtoList(IEnumerable<TrendyolProductDto> products)
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
