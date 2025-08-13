using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Services.Commerce;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Commerce.Smartstore
{
    public class SmartstoreProductWriter : ICommerceProductWriter
    {
        private readonly SmartstoreClient _smartstoreClient;
        private readonly ILogger<SmartstoreProductWriter> _logger;
        public SmartstoreProductWriter(SmartstoreClient smartstoreClient, ILogger<SmartstoreProductWriter> logger)
        {
            _smartstoreClient = smartstoreClient;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DeleteProductAsync(string sku)
        {
            await _smartstoreClient.DeleteProductAsync(sku);
        }

        public async Task DeleteProductsAsync(IEnumerable<string> skus)
        {
            await _smartstoreClient.DeleteProductsAsync(skus);
        }

        public async Task UpsertProductAsync(ProductDto product)
        {
            await _smartstoreClient.UpsertProductAsync(product);
        }

        public async Task UpsertProductsAsync(IEnumerable<ProductDto> products)
        {
            await _smartstoreClient.UpsertProductsAsync(products);
        }
    }
}
