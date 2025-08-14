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

        public async Task<BrandDto?> BrandExistsAsync(string brandName)
        {
            return await _smartstoreClient.BrandExistsAsync(brandName);
        }

        public Task<int> CreateBrandAsync(BrandDto brand)
        {
            return _smartstoreClient.CreateBrandAsync(brand);
        }

        public Task DeleteBrandAsync(int brandId)
        {
           return _smartstoreClient.DeleteBrandAsync(brandId);
        }

        public Task UpdateBrandAsync(BrandDto brand,int id)
        {
           return _smartstoreClient.UpdateBrandAsync(brand,id);
        }
    }
}
