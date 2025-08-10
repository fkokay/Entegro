using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreManufacturerMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static BrandDto? ToDto(SmartstoreManufacturerDto smartstoreManufacturer)
        {
            try
            {
                if (smartstoreManufacturer == null)
                {
                    return null;
                }

                BrandDto brand = new BrandDto();
                brand.MetaTitle = smartstoreManufacturer.MetaTitle;
                brand.MetaDescription = smartstoreManufacturer.MetaDescription;
                brand.MetaKeywords = smartstoreManufacturer.MetaKeywords;
                brand.Name = smartstoreManufacturer.Name;
                brand.Description = smartstoreManufacturer.Description;
                brand.DisplayOrder = smartstoreManufacturer.DisplayOrder;
                brand.CreatedOn = DateTime.Now;
                brand.UpdatedOn = DateTime.Now;


                return brand; ;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Manufacturer mapping sırasında hata oluştu. ManufacturerId: {ManufacturerId}", smartstoreManufacturer.Id);
                return null;
            }
        }

        public static IEnumerable<BrandDto> ToDtoList(IEnumerable<SmartstoreManufacturerDto> manufacturers)
        {
            if (manufacturers == null)
                yield break;

            foreach (var manufacturer in manufacturers)
            {
                var dto = ToDto(manufacturer);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
