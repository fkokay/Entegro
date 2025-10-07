using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.CrossSellProduct;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class CrossSellProductService : ICrossSellProductService
    {
        private readonly ICrossSellProductRepository _crossSellProductRepository;
        private readonly IMapper _mapper;
        public CrossSellProductService(ICrossSellProductRepository crossSellProductRepository, IMapper mapper)
        {
            _crossSellProductRepository = crossSellProductRepository ?? throw new ArgumentNullException(nameof(crossSellProductRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CrossSellProductDto> AddAsync(CreateCrossSellProductDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var crossSellProduct = _mapper.Map<CrossSellProduct>(model);
            await _crossSellProductRepository.AddAsync(crossSellProduct);

            return _mapper.Map<CrossSellProductDto>(crossSellProduct);
        }

        public async Task DeleteAllAsync(List<int> idList)
        {
            if (idList == null || !idList.Any())
                throw new ArgumentException("Silinecek ID listesi boş olamaz.", nameof(idList));

            var crossSellProductToDelete = new List<CrossSellProduct>();

            foreach (var id in idList)
            {
                if (id <= 0)
                    continue;

                var crossSellProduct = await _crossSellProductRepository.GetByIdAsync(id);
                if (crossSellProduct != null)
                {
                    crossSellProductToDelete.Add(crossSellProduct);
                }
            }

            if (!crossSellProductToDelete.Any())
                throw new KeyNotFoundException("Hiçbir geçerli CrossSellProduct bulunamadı.");
            await _crossSellProductRepository.DeleteAllAsync(crossSellProductToDelete);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var crossSellProduct = await _crossSellProductRepository.GetByIdAsync(id);
            if (crossSellProduct == null)
                throw new KeyNotFoundException($"ID {id} ile ilişki bulunamadı.");

            await _crossSellProductRepository.DeleteAsync(crossSellProduct);
        }

        public async Task<bool> ExistsByIdAsync(int productId1, int productId2)
        {
            if (productId1 <= 0 && productId2 <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(productId1), nameof(productId2));
            }

            return await _crossSellProductRepository.ExistsByIdAsync(productId1, productId2);
        }

        public async Task<CrossSellProductDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var crossSellProduct = await _crossSellProductRepository.GetByIdAsync(id);
            if (crossSellProduct == null)
            {
                return null;
            }
            var crossSellProductDto = _mapper.Map<CrossSellProductDto>(crossSellProduct);
            return crossSellProductDto;
        }

        public async Task<CrossSellProductDto?> GetByIdAsync(int productId1, int productId2)
        {
            if (productId1 <= 0 && productId2 <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(productId1), nameof(productId2));
            }

            var crossSellProduct = await _crossSellProductRepository.GetByIdAsync(productId1, productId2);
            if (crossSellProduct == null)
            {
                return null;
            }
            var crossSellProductDto = _mapper.Map<CrossSellProductDto>(crossSellProduct);
            return crossSellProductDto;
        }

        public async Task<PagedResult<CrossSellProductDto>> GetPagedAsync(GridCommand gridCommand, int productId)
        {
            var crossSellProduct = await _crossSellProductRepository.GetPagedAsync(gridCommand, productId);

            var items = await crossSellProduct.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<CrossSellProductDto>(x);
                return model;
            }).AsyncToList();

            return new PagedResult<CrossSellProductDto>
            {
                Items = items,
                TotalCount = crossSellProduct.TotalCount,
                PageNumber = crossSellProduct.PageNumber,
                PageSize = crossSellProduct.PageSize
            };
        }

        public async Task<CrossSellProductDto> UpdateAsync(UpdateCrossSellProductDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingCrossSellProduct = await _crossSellProductRepository.GetByIdAsync(model.Id);
            if (existingCrossSellProduct == null)
                throw new KeyNotFoundException($"ID {model.Id} ile CrossSellProduct bulunamadı.");

            _mapper.Map(model, existingCrossSellProduct);
            await _crossSellProductRepository.UpdateAsync(existingCrossSellProduct);

            return _mapper.Map<CrossSellProductDto>(existingCrossSellProduct);
        }
    }
}
