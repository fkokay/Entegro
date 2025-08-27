using AutoMapper;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class ProductCategoryMappingService : IProductCategoryMappingService
    {
        private readonly IProductCategoryMappingRepository _productCategoryMappingRepository;
        private readonly ICategoryRepository _catRepo;
        private readonly IMapper _mapper;
        public ProductCategoryMappingService(IProductCategoryMappingRepository productCategoryMappingRepository, IMapper mapper, ICategoryRepository catRepo)
        {
            _productCategoryMappingRepository = productCategoryMappingRepository ?? throw new ArgumentNullException(nameof(productCategoryMappingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _catRepo = catRepo;
        }
        public async Task<ProductCategoryDto> CreateProductCategoryAsync(CreateProductCategoryDto createProductCategoryDto)
        {
            var createProductCategory = _mapper.Map<ProductCategory>(createProductCategoryDto);
            await _productCategoryMappingRepository.AddAsync(createProductCategory);
            return _mapper.Map<ProductCategoryDto>(createProductCategory);
        }

        public async Task DeleteProductCategoryAsync(int productCategoryId)
        {
            var productCategory = await _productCategoryMappingRepository.GetByIdAsync(productCategoryId);

            if (productCategory == null)
            {
                throw new KeyNotFoundException($"ProductCategory with ID {productCategoryId} not found.");
            }
            await _productCategoryMappingRepository.DeleteAsync(productCategory);
        }

        public async Task<IReadOnlyList<ProductCategoryPathDto>> GetCategoryPathsByProductAsync(int productId)
        {
            var mappings = await _productCategoryMappingRepository.GetByProductWithCategoryAsync(productId);

            var allPathIds = mappings.SelectMany(m => SplitTreePathIds(m.Category?.TreePath)).Distinct().ToArray();

            var nameDict = await _catRepo.GetNamesByIdsAsync(allPathIds);

            return mappings
                .Select(m => new ProductCategoryPathDto
                {
                    Id = m.Id,
                    ProductId = m.ProductId,
                    CategoryId = m.CategoryId,
                    DisplayOrder = m.DisplayOrder,
                    CategoryPath = BuildPathString(m.Category?.TreePath, nameDict)
                })
                .OrderBy(d => d.DisplayOrder)
                .ThenBy(d => d.CategoryPath)
                .ToList();
        }

        public async Task<IReadOnlyDictionary<int, IReadOnlyList<ProductCategoryPathDto>>> GetCategoryPathsByProductsAsync(IEnumerable<int> productIds)
        {
            var mappings = await _productCategoryMappingRepository.GetByProductsWithCategoryAsync(productIds);

            var allPathIds = mappings.SelectMany(m => SplitTreePathIds(m.Category?.TreePath)).Distinct().ToArray();

            var nameDict = await _catRepo.GetNamesByIdsAsync(allPathIds);

            var dtos = mappings.Select(m => new ProductCategoryPathDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                CategoryId = m.CategoryId,
                DisplayOrder = m.DisplayOrder,
                CategoryPath = BuildPathString(m.Category?.TreePath, nameDict)
            });

            return dtos
                .GroupBy(d => d.ProductId)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<ProductCategoryPathDto>)g
                        .OrderBy(x => x.DisplayOrder)
                        .ThenBy(x => x.CategoryPath)
                        .ToList()
                );
        }

        public async Task<ProductCategoryDto> GetProductCategoryByIdAsync(int productCategoryId)
        {
            var productCategory = await _productCategoryMappingRepository.GetByIdAsync(productCategoryId);
            if (productCategory == null)
            {
                throw new KeyNotFoundException($"Brand with ID {productCategoryId} not found.");
            }

            var productCategoryDto = _mapper.Map<ProductCategoryDto>(productCategory);
            return productCategoryDto;
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetProductCategorysAsync()
        {
            var productCategories = await _productCategoryMappingRepository.GetAllAsync();
            var productCategoryDtos = _mapper.Map<IEnumerable<ProductCategoryDto>>(productCategories);
            return productCategoryDtos;
        }

        public async Task<ProductCategoryDto> UpdateProductCategoryAsync(UpdateProductCategoryDto updateProductCategory)
        {
            await _productCategoryMappingRepository.UpdateAsync(_mapper.Map<ProductCategory>(updateProductCategory));
            return _mapper.Map<ProductCategoryDto>(updateProductCategory);
        }
        private static IReadOnlyList<int> SplitTreePathIds(string? treePath)
        {
            if (string.IsNullOrWhiteSpace(treePath))
                return Array.Empty<int>();

            var normalized = treePath.Replace('>', '/').Replace('\\', '/').Replace('|', '/');
            return normalized.Split('/', StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => int.TryParse(s, out var id) ? id : (int?)null)
                             .Where(id => id.HasValue)
                             .Select(id => id!.Value)
                             .ToArray();
        }

        private static string BuildPathString(string? treePath, IReadOnlyDictionary<int, string> nameDict)
        {
            var ids = SplitTreePathIds(treePath);
            var names = ids.Select(id => nameDict.TryGetValue(id, out var nm) ? nm : $"#{id}");
            return string.Join(" > ", names);
        }
    }
}
