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
                brand.Id = smartstoreManufacturer.Id;
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
        public static SmartstoreManufacturerDto? ToDto(BrandDto brand)
        {
            try
            {
                if (brand == null)
                {
                    return null;
                }

                SmartstoreManufacturerDto smartstoreManufacturer = new SmartstoreManufacturerDto();
                smartstoreManufacturer.Id = 0;
                smartstoreManufacturer.MetaTitle = brand.MetaTitle;
                smartstoreManufacturer.MetaDescription = brand.MetaDescription;
                smartstoreManufacturer.MetaKeywords = brand.MetaKeywords;
                smartstoreManufacturer.Name = brand.Name;
                smartstoreManufacturer.Description = brand.Description;
                smartstoreManufacturer.DisplayOrder = brand.DisplayOrder;
                smartstoreManufacturer.CreatedOnUtc = DateTime.UtcNow;
                smartstoreManufacturer.UpdatedOnUtc = DateTime.UtcNow;
                smartstoreManufacturer.BottomDescription = "";
                smartstoreManufacturer.ManufacturerTemplateId = 1;
                smartstoreManufacturer.PageSize = 48;
                smartstoreManufacturer.AllowCustomersToSelectPageSize = false;
                smartstoreManufacturer.DisplayOrder = brand.DisplayOrder;
                smartstoreManufacturer.Published = true;
                smartstoreManufacturer.HasDiscountsApplied = false;
                smartstoreManufacturer.SubjectToAcl = false;
                smartstoreManufacturer.LimitedToStores = false;
                smartstoreManufacturer.PageSizeOptions = "48,96,144";
                smartstoreManufacturer.CreatedOnUtc = DateTime.UtcNow;
                smartstoreManufacturer.UpdatedOnUtc = DateTime.UtcNow;

                return smartstoreManufacturer;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Manufacturer mapping sırasında hata oluştu. ManufacturerId: {ManufacturerId}", brand.Id);
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
