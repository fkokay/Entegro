using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.Trendyol
{
    public class TrendyolBrandMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static BrandDto? ToDto(TrendyolBrandDto trendyolBrand)
        {
            if (trendyolBrand == null)
            {
                return null;
            }

            BrandDto brand = new BrandDto();
            brand.Id = trendyolBrand.Id;
            brand.Name = trendyolBrand.Name;
            brand.Description = string.Empty;
            brand.MetaTitle = trendyolBrand.Name; 
            brand.MetaDescription = trendyolBrand.Name; 
            brand.MetaKeywords = trendyolBrand.Name; 
            brand.DisplayOrder = 0; 
            brand.CreatedOn = DateTime.Now;
            brand.UpdatedOn = DateTime.Now;


            return brand;
        }

        public static IEnumerable<BrandDto> ToDtoList(IEnumerable<TrendyolBrandDto> brands)
        {
            if (brands == null)
                yield break;

            foreach (var brand in brands)
            {
                var dto = ToDto(brand);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
