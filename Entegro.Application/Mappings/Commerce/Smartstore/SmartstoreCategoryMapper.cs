using Entegro.Application.DTOs.Brand;
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
                categoryDto.Id = smartstoreCategory.Id;
                categoryDto.ParentCategoryId = null;
                categoryDto.Name = smartstoreCategory.Name;
                categoryDto.DisplayOrder = smartstoreCategory.DisplayOrder;
                categoryDto.MetaTitle = smartstoreCategory.MetaTitle;
                categoryDto.MetaKeywords = smartstoreCategory.MetaKeywords;
                categoryDto.MetaDescription = smartstoreCategory.MetaDescription;
                categoryDto.CreatedOn = smartstoreCategory.CreatedOnUtc;
                categoryDto.UpdatedOn = smartstoreCategory.UpdatedOnUtc;
                
                return categoryDto; ;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Category mapping sırasında hata oluştu. CategoryId: {CategoryId}", smartstoreCategory.Id);
                return null;
            }
        }

        public static SmartstoreCategoryDto? ToDto(CategoryDto category)
        {
            try
            {
                if (category == null)
                {
                    return null;
                }

                SmartstoreCategoryDto smartstoreCategory = new SmartstoreCategoryDto();
                smartstoreCategory.Alias = "";
                smartstoreCategory.AllowCustomersToSelectPageSize = false;
                smartstoreCategory.BadgeStyle = 0;
                smartstoreCategory.BadgeText = "";
                smartstoreCategory.BottomDescription = "";
                smartstoreCategory.CategoryTemplateId = 0;
                smartstoreCategory.CreatedOnUtc = DateTime.Now;
                smartstoreCategory.TreePath = "/";
                smartstoreCategory.FullName = category.Name;
                smartstoreCategory.DefaultViewMode = "Grid";
                smartstoreCategory.Description = "";
                smartstoreCategory.DisplayOrder = 0;
                smartstoreCategory.UpdatedOnUtc = DateTime.Now;
                smartstoreCategory.SubjectToAcl = false;
                smartstoreCategory.ShowOnHomePage = false;
                smartstoreCategory.Published = true;
                smartstoreCategory.ParentId = null;
                smartstoreCategory.PageSizeOptions = "10,20,50";
                smartstoreCategory.PageSize = 20;
                smartstoreCategory.Name = category.Name;
                smartstoreCategory.MetaTitle = category.Name;
                smartstoreCategory.MetaKeywords = "";
                smartstoreCategory.MetaDescription = "";
                smartstoreCategory.MediaFileId = null;
                smartstoreCategory.LimitedToStores = false;
                smartstoreCategory.IgnoreInMenus = false;
                smartstoreCategory.Id = 0;
                smartstoreCategory.HasDiscountsApplied = false;
                smartstoreCategory.ExternalLink = "";

                return smartstoreCategory;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Category mapping sırasında hata oluştu. CategoryId: {CategoryId}", category.Id);
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
