using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreCategoryMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static CategoryDto? ToDto(SmartstoreCategoryDto smartstoreCategory)
        {
            try
            {
                if (smartstoreCategory == null)
                {
                    return null;
                }

                CategoryDto categoryDto = new CategoryDto();
                
                return categoryDto; ;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Category mapping sırasında hata oluştu. CategoryId: {CategoryId}", smartstoreCategory.Id);
                return null;
            }
        }

        public static IEnumerable<CategoryDto> ToDtoList(IEnumerable<SmartstoreCategoryDto> categories)
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
