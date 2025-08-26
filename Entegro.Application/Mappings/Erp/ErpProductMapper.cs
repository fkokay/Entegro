using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.DTOs.ProductVariantAttribute;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;
using Entegro.Application.DTOs.ProductVariantAttributeValue;
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
            product.Description = erpProduct.Description;
            product.Price = erpProduct.Price;
            product.Currency = erpProduct.Currency;
            product.Unit = erpProduct.Unit;
            product.VatRate = erpProduct.VatRate;
            product.VatInc = erpProduct.VatInc;

            if (!string.IsNullOrEmpty(erpProduct.BrandName))
            {
                product.Brand = new DTOs.Brand.BrandDto();
                product.Brand.Name = erpProduct.BrandName;
                product.Brand.DisplayOrder = 0;
                product.Brand.CreatedOn = DateTime.Now;
                product.Brand.UpdatedOn = DateTime.Now;
                product.Brand.MediaFileId = null;
                product.Brand.MetaKeywords = "";
                product.Brand.MetaTitle = "";
                product.Brand.MetaDescription = "";
            }

            if (!string.IsNullOrEmpty(erpProduct.Category1))
            {
                var categoryHierarchy = BuildCategoryHierarchy(
                    erpProduct.Category1,
                    erpProduct.Category2,
                    erpProduct.Category3,
                    erpProduct.Category4
                );

                product.ProductCategories.Add(new DTOs.ProductCategory.ProductCategoryDto
                {
                    Category = categoryHierarchy,
                    DisplayOrder = 0,
                    CategoryId = 0,
                    ProductId = 0
                });
            }

            return product;
        }

        private static DTOs.Category.CategoryDto BuildCategoryHierarchy(params string[] categories)
        {
            DTOs.Category.CategoryDto root = null;
            DTOs.Category.CategoryDto current = null;

            foreach (var categoryName in categories.Where(c => !string.IsNullOrEmpty(c)))
            {
                var newCategory = new DTOs.Category.CategoryDto
                {
                    Name = categoryName,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    Description = "",
                    DisplayOrder = 0,
                    MetaKeywords = "",
                    MetaDescription = "",
                    MetaTitle = "",
                    TreePath = "",
                    SubCategories = new List<DTOs.Category.CategoryDto>()
                };

                if (root == null)
                {
                    root = newCategory;
                    current = root;
                }
                else
                {
                    current.SubCategories.Add(newCategory);
                    current = newCategory;
                }
            }

            return root;
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
