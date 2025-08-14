using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.CategoryAttribute;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.Trendyol
{
    public class TrendyolCategoryAttributeMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static CategoryAttributeDto? ToDto(TrendyolCategoryWithAttributeDto trendyolCategoryWithAttribute)
        {
            if (trendyolCategoryWithAttribute == null)
            {
                return null;
            }

            CategoryAttributeDto categoryAttribute = new CategoryAttributeDto();



            return categoryAttribute;
        }
    }
}
