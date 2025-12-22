using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.ReturnRequestItem;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Checkout;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Base
{
    public class ReturnRequestItemService : IReturnRequestItemService
    {
        private readonly IReturnRequestItemRepository _returnRequestItemRepository;
        private readonly IMapper _mapper;
        public ReturnRequestItemService(IReturnRequestItemRepository returnRequestItemRepository, IMapper mapper)
        {
            _returnRequestItemRepository = returnRequestItemRepository ?? throw new ArgumentNullException(nameof(returnRequestItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<ReturnRequestItemDto>> GetAllWithIntegrationSkuAsync(string integrationSku)
        {
            var returnRequestItems = await _returnRequestItemRepository.GetAllIntegrationSkuWithAsync(integrationSku);
            var returnRequestItemDtos = _mapper.Map<IEnumerable<ReturnRequestItemDto>>(returnRequestItems);
            return returnRequestItemDtos.ToList();
        }

        public async Task<ReturnRequestItemDto?> GetByIdAsync(int id)
        {
            return await _returnRequestItemRepository.GetByIdAsync(id) is ReturnRequestItem returnRequestItem ? _mapper.Map<ReturnRequestItemDto>(returnRequestItem) : null;
        }

        public async Task<ReturnRequestItemDto> UpdateAsync(UpdateReturnRequestItemDto updateReturnRequestItem)
        {
            if (updateReturnRequestItem == null)
                throw new ArgumentNullException(nameof(updateReturnRequestItem));

            var existinReturnRequestItem = await _returnRequestItemRepository.GetByIdAsync(updateReturnRequestItem.Id);
            if (existinReturnRequestItem == null)
                throw new KeyNotFoundException($"ID {existinReturnRequestItem.Id} ile ReturnRequest bulunamadı.");

            existinReturnRequestItem.ProductId = updateReturnRequestItem.ProductId;
            existinReturnRequestItem.ProductName = updateReturnRequestItem.ProductName;
            existinReturnRequestItem.Barcode = updateReturnRequestItem.Barcode;

            await _returnRequestItemRepository.UpdateAsync(existinReturnRequestItem);
            return _mapper.Map<ReturnRequestItemDto>(existinReturnRequestItem);
        }
    }
}
