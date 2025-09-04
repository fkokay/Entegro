using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.ProductBrand;
using Entegro.Application.DTOs.ProductCategory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreProductManufacturerMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static ProductBrandDto? ToDto(SmartstoreProductManufacturerDto smartstoreProductManufacturer)
        {
            try
            {
                if (smartstoreProductManufacturer == null)
                {
                    return null;
                }

                ProductBrandDto productManufacturer = new ProductBrandDto();
                productManufacturer.Id = smartstoreProductManufacturer.Id;
                productManufacturer.ProductId = smartstoreProductManufacturer.ProductId;
                productManufacturer.ManufacturerId = smartstoreProductManufacturer.ManufacturerId;
                productManufacturer.IsFeaturedProduct  = smartstoreProductManufacturer.IsFeaturedProduct;
                productManufacturer.DisplayOrder = smartstoreProductManufacturer.DisplayOrder;

                return productManufacturer;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Category mapping sırasında hata oluştu. CategoryId: {CategoryId}", smartstoreProductManufacturer.Id);
                return null;
            }
        }

        public static SmartstoreProductManufacturerDto? ToDto(ProductBrandDto productCategory)
        {
            try
            {
                if (productCategory == null)
                {
                    return null;
                }

                SmartstoreProductManufacturerDto smartstoreProductManufacturer = new SmartstoreProductManufacturerDto();
                smartstoreProductManufacturer.Id = productCategory.Id;
                smartstoreProductManufacturer.ProductId = productCategory.ProductId;
                smartstoreProductManufacturer.ManufacturerId= productCategory.ManufacturerId;
                smartstoreProductManufacturer.IsFeaturedProduct= productCategory.IsFeaturedProduct;
                smartstoreProductManufacturer.DisplayOrder= productCategory.DisplayOrder;

                return smartstoreProductManufacturer;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Category mapping sırasında hata oluştu. CategoryId: {CategoryId}", productCategory.Id);
                return null;
            }
        }

        public static IEnumerable<ProductBrandDto> ToDtoList(IEnumerable<SmartstoreProductManufacturerDto> smartstoreProductManufacturers)
        {
            if (smartstoreProductManufacturers == null)
                yield break;

            foreach (var smartstoreProductManufacturer in smartstoreProductManufacturers)
            {
                var dto = ToDto(smartstoreProductManufacturer);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
