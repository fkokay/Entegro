using AutoMapper;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Service.Jobs
{
    public class SmartstoreDataSyncJob : IJob
    {
        private readonly ISmartstoreService _smartstoreService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public SmartstoreDataSyncJob(ISmartstoreService smartstoreService,IProductService productService,IMapper mapper)
        {
            _smartstoreService = smartstoreService ?? throw new ArgumentNullException(nameof(smartstoreService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var smartstoreProducts = await _smartstoreService.GetProductsAsync();
            if (smartstoreProducts == null || !smartstoreProducts.Any())
            {
                Console.WriteLine("No products found in Smartstore.");
                return;
            }

            var products = _mapper.Map<IEnumerable<Entegro.Application.DTOs.Product.CreateProductDto>>(smartstoreProducts);
            foreach (var product in products)
            {
                try
                {
                    await _productService.CreateProductAsync(product);
                    Console.WriteLine($"Product {product.Name} created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating product {product.Name}: {ex.Message}");
                }
            }
        }
    }
}
