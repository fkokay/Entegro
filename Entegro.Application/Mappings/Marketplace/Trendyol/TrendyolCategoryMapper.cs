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
    public class TrendyolCategoryMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static CategoryDto? ToDto(TrendyolCategoryDto trendyolCategory)
        {
            if (trendyolCategory == null)
            {
                return null;
            }


            CategoryDto category = new CategoryDto();
            category.Id = trendyolCategory.Id;
            category.Name = trendyolCategory.Name;
            category.ParentCategoryId = trendyolCategory.ParentId ?? 0;
            category.SubCategories = new List<CategoryDto>();
            if (trendyolCategory.SubCategories != null && trendyolCategory.SubCategories.Any())
            {
                category.SubCategories = ToDtoList(trendyolCategory.SubCategories).ToList();
            }


            return category;
        }

        public static IEnumerable<CategoryDto> ToDtoList(IEnumerable<TrendyolCategoryDto> categories)
        {
            if (categories == null)
                yield break;

            foreach (var category in categories)
            {
                var dto = ToDto(category);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
