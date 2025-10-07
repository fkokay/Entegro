using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.RelatedProduct;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class RelatedProductService : IRelatedProductService
    {
        private readonly IRelatedProductRepository _relatedProductRepository;
        private readonly IMapper _mapper;
        public RelatedProductService(IRelatedProductRepository relatedProductRepository, IMapper mapper)
        {
            _relatedProductRepository = relatedProductRepository ?? throw new ArgumentNullException(nameof(relatedProductRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<RelatedProductDto> AddAsync(CreateRelatedProductDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var relatedProduct = _mapper.Map<RelatedProduct>(model);
            await _relatedProductRepository.AddAsync(relatedProduct);

            return _mapper.Map<RelatedProductDto>(relatedProduct);
        }

        public async Task DeleteAllAsync(List<int> idList)
        {
            if (idList == null || !idList.Any())
                throw new ArgumentException("Silinecek ID listesi boş olamaz.", nameof(idList));

            var relatedProductToDelete = new List<RelatedProduct>();

            foreach (var id in idList)
            {
                if (id <= 0)
                    continue;

                var relatedProduct = await _relatedProductRepository.GetByIdAsync(id);
                if (relatedProduct != null)
                {
                    relatedProductToDelete.Add(relatedProduct);
                }
            }

            if (!relatedProductToDelete.Any())
                throw new KeyNotFoundException("Hiçbir geçerli  RelatedProduct bulunamadı.");
            await _relatedProductRepository.DeleteAllAsync(relatedProductToDelete);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var relatedProduct = await _relatedProductRepository.GetByIdAsync(id);
            if (relatedProduct == null)
                throw new KeyNotFoundException($"ID {id} ile ilişki bulunamadı.");

            await _relatedProductRepository.DeleteAsync(relatedProduct);
        }

        public async Task<bool> ExistsByIdAsync(int productId1, int productId2)
        {
            if (productId1 <= 0 && productId2 <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(productId1), nameof(productId2));
            }

            return await _relatedProductRepository.ExistsByIdAsync(productId1, productId2);
        }

        public async Task<RelatedProductDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var relatedProduct = await _relatedProductRepository.GetByIdAsync(id);
            if (relatedProduct == null)
            {
                return null;
            }
            var relatedProductDto = _mapper.Map<RelatedProductDto>(relatedProduct);
            return relatedProductDto;
        }

        public async Task<RelatedProductDto?> GetByIdAsync(int productId1, int productId2)
        {
            if (productId1 <= 0 && productId2 <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(productId1), nameof(productId2));
            }

            var relatedProduct = await _relatedProductRepository.GetByIdAsync(productId1, productId2);
            if (relatedProduct == null)
            {
                return null;
            }
            var relatedProductDto = _mapper.Map<RelatedProductDto>(relatedProduct);
            return relatedProductDto;
        }

        public async Task<PagedResult<RelatedProductDto>> GetPagedAsync(GridCommand gridCommand, int productId)
        {
            var relatedProduct = await _relatedProductRepository.GetPagedAsync(gridCommand, productId);

            var items = await relatedProduct.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<RelatedProductDto>(x);
                return model;
            }).AsyncToList();

            return new PagedResult<RelatedProductDto>
            {
                Items = items,
                TotalCount = relatedProduct.TotalCount,
                PageNumber = relatedProduct.PageNumber,
                PageSize = relatedProduct.PageSize
            };
        }

        public async Task<RelatedProductDto> UpdateAsync(UpdateRelatedProductDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingRelatedProduct = await _relatedProductRepository.GetByIdAsync(model.Id);
            if (existingRelatedProduct == null)
                throw new KeyNotFoundException($"ID {model.Id} ile  RelatedProduct bulunamadı.");

            _mapper.Map(model, existingRelatedProduct);
            await _relatedProductRepository.UpdateAsync(existingRelatedProduct);

            return _mapper.Map<RelatedProductDto>(existingRelatedProduct);
        }
    }
}

