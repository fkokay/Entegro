using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.Interfaces.Services.Commerce;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Commerce.Smartstore
{
    public class SmartstoreManufacturerWriter : ICommerceBrandWriter
    {
        private readonly SmartstoreClient _smartstoreClient;
        private readonly ILogger<SmartstoreManufacturerWriter> _logger;
        public SmartstoreManufacturerWriter(SmartstoreClient smartstoreClient, ILogger<SmartstoreManufacturerWriter> logger)
        {
            _smartstoreClient = smartstoreClient;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BrandDto?> BrandExistsAsync(SmartstoreApiContext context, string brandName)
        {
            return await _smartstoreClient.BrandExistsAsync(context, brandName);
        }

        public Task<int> CreateBrandAsync(SmartstoreApiContext context, BrandDto brand)
        {
            return _smartstoreClient.CreateBrandAsync(context, brand);
        }

        public Task DeleteBrandAsync(SmartstoreApiContext context, int brandId)
        {
           return _smartstoreClient.DeleteBrandAsync(context, brandId);
        }

        public Task UpdateBrandAsync(SmartstoreApiContext context, BrandDto brand,int id)
        {
           return _smartstoreClient.UpdateBrandAsync(context, brand, id);
        }
    }
}
