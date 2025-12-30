using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Mappings.Marketplace.Trendyol;
using Entegro.Domain.Entities.Catalog;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.Pazarama
{
    public class PazaramaProductMapper
    {
        private static ILogger? _logger;
        private static IBrandService _brandService;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static void ConfigureBrandService(IBrandService brandService)
        {
            _brandService = brandService;
        }


        public static ProductDto? ToDto(PazaramaProductDto pazaramaProduct)
        {
            if (pazaramaProduct == null)
            {
                return null;
            }
            ProductDto productDto = new ProductDto();
            productDto.Name = pazaramaProduct.Name;
            productDto.Code = pazaramaProduct.Code;
            productDto.Description = pazaramaProduct.Description!=null ? pazaramaProduct.Description.ToString() : "";
            productDto.Price = Convert.ToDecimal(pazaramaProduct.ListPrice);
            productDto.SalePrice = Convert.ToDecimal(pazaramaProduct.SalePrice);
            productDto.MetaKeywords = "";
            productDto.MetaDescription = "";
            productDto.MetaTitle = "";
            productDto.StockQuantity = pazaramaProduct.StockCount;
            productDto.Barcode = pazaramaProduct.Code;

            var existBrand = _brandService.ExistsByNameAsync(pazaramaProduct.BrandName).GetAwaiter().GetResult();

            if (existBrand)
            {
                var brand = _brandService.GetByNameAsync(pazaramaProduct.BrandName).GetAwaiter().GetResult();

                if (brand != null)
                    productDto.BrandId = brand.Id;
            }
            else
            {
                productDto.Brand = new BrandDto()
                {
                    Name = pazaramaProduct.BrandName,
                    Description = "",
                    MetaDescription = "",
                    MetaTitle = pazaramaProduct.BrandName,
                    MetaKeywords = pazaramaProduct.BrandName.ToLower(),
                    DisplayOrder = 0,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                };
            }

            return productDto;
        }

        public static IEnumerable<ProductDto> ToDtoList(IEnumerable<PazaramaProductDto> products)
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
