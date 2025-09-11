using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.ProductMediaFile;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreProductMediaFileMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static ProductMediaFileDto? ToDto(SmartstoreProductMediaFileDto smartstoreProductMediaFile)
        {
            try
            {
                if (smartstoreProductMediaFile == null)
                {
                    return null;
                }

                ProductMediaFileDto productMediaFile = new ProductMediaFileDto();
                productMediaFile.ProductId= smartstoreProductMediaFile.ProductId;
                productMediaFile.Id = smartstoreProductMediaFile.Id;
                productMediaFile.MediaFileId = smartstoreProductMediaFile.MediaFileId;
                productMediaFile.DisplayOrder = smartstoreProductMediaFile.DisplayOrder;



                return productMediaFile;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Manufacturer mapping sırasında hata oluştu. ManufacturerId: {ManufacturerId}", smartstoreProductMediaFile.Id);
                return null;
            }
        }
        public static SmartstoreProductMediaFileDto? ToDto(ProductMediaFileDto productMediaFile)
        {
            try
            {
                if (productMediaFile == null)
                {
                    return null;
                }

                SmartstoreProductMediaFileDto smartstoreProductMediaFile = new SmartstoreProductMediaFileDto();
                smartstoreProductMediaFile.ProductId = productMediaFile.ProductId;
                smartstoreProductMediaFile.MediaFileId = productMediaFile.MediaFileId;
                smartstoreProductMediaFile.DisplayOrder = productMediaFile.DisplayOrder;
                smartstoreProductMediaFile.Id = productMediaFile.Id;
                

                return smartstoreProductMediaFile;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Manufacturer mapping sırasında hata oluştu. ManufacturerId: {ManufacturerId}", productMediaFile.Id);
                return null;
            }
        }

        public static IEnumerable<ProductMediaFileDto> ToDtoList(IEnumerable<SmartstoreProductMediaFileDto> smartstoreProductMediaFiles)
        {
            if (smartstoreProductMediaFiles == null)
                yield break;

            foreach (var smartstoreProductMediaFile in smartstoreProductMediaFiles)
            {
                var dto = ToDto(smartstoreProductMediaFile);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
