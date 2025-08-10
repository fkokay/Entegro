using AutoMapper;
using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IBrandService brandService, IMapper mapper)
        {
            _productRepository = productRepository;
            _brandService = brandService;
            _mapper = mapper;
        }
        public async Task<int> CreateProductAsync(CreateProductDto createProduct)
        {
            if (createProduct.BrandId == null && createProduct.Brand != null)
            {
                if (await _brandService.ExistsByNameAsync(createProduct.Brand.Name))
                {
                    var brand = await _brandService.GetBrandByNameAsync(createProduct.Brand.Name);
                    createProduct.BrandId = brand.Id;
                    createProduct.Brand = null;
                }
                else
                {
                    var createBrand = _mapper.Map<CreateBrandDto>(createProduct.Brand);

                    var brandResult = await _brandService.CreateBrandAsync(createBrand);
                    createProduct.BrandId = brandResult;
                    createProduct.Brand = null;
                }
            }

            var product = _mapper.Map<Product>(createProduct);
            await _productRepository.AddAsync(product);

            return product.Id;
        }


        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }
            await _productRepository.DeleteAsync(product);
            return true;
        }

        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            return await _productRepository.ExistsByCodeAsync(productCode);
        }

        public async Task<bool> ExistsByNameAsync(string productName)
        {
            return await _productRepository.ExistsByNameAsync(productName);
        }

        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return productDtos;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize)
        {
            var products = await _productRepository.GetAllAsync(pageNumber, pageSize);
            var productDtos = _mapper.Map<PagedResult<ProductDto>>(products);
            return productDtos;
        }

        public async Task<bool> UpdateProductAsync(UpdateProductDto updateProduct)
        {
            await _productRepository.UpdateAsync(_mapper.Map<Product>(updateProduct));
            return true;
        }
    }
}
