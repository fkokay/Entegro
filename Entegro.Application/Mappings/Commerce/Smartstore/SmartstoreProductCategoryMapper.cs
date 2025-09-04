using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.ProductCategory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreProductCategoryMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static ProductCategoryDto? ToDto(SmartstoreProductCategoryDto smartstoreProductCategory)
        {
            try
            {
                if (smartstoreProductCategory == null)
                {
                    return null;
                }

                ProductCategoryDto productCategory = new ProductCategoryDto();
                productCategory.Id = smartstoreProductCategory.Id;
                productCategory.CategoryId = smartstoreProductCategory.CategoryId;
                productCategory.ProductId = smartstoreProductCategory.ProductId;
                productCategory.DisplayOrder = smartstoreProductCategory.DisplayOrder;

                return productCategory; ;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Category mapping sırasında hata oluştu. CategoryId: {CategoryId}", smartstoreProductCategory.Id);
                return null;
            }
        }

        public static SmartstoreProductCategoryDto? ToDto(ProductCategoryDto productCategory)
        {
            try
            {
                if (productCategory == null)
                {
                    return null;
                }

                SmartstoreProductCategoryDto smartstoreProductCategory = new SmartstoreProductCategoryDto();
                smartstoreProductCategory.Id = productCategory.Id;
                smartstoreProductCategory.ProductId = productCategory.ProductId;
                smartstoreProductCategory.CategoryId = productCategory.CategoryId;
                smartstoreProductCategory.DisplayOrder= productCategory.DisplayOrder;
                smartstoreProductCategory.IsFeaturedProduct = false;
                smartstoreProductCategory.IsSystemMapping = false;

                return smartstoreProductCategory;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Category mapping sırasında hata oluştu. CategoryId: {CategoryId}", productCategory.Id);
                return null;
            }
        }

        public static IEnumerable<ProductCategoryDto> ToDtoList(IEnumerable<SmartstoreProductCategoryDto> smartstoreProductCategories)
        {
            if (smartstoreProductCategories == null)
                yield break;

            foreach (var smartstoreProductCategory in smartstoreProductCategories)
            {
                var dto = ToDto(smartstoreProductCategory);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
