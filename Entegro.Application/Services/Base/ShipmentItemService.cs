using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ShipmentItem;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Checkout;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class ShipmentItemService : IShipmentItemService
    {
        private readonly IShipmentItemRepository _shipmentItemRepository;
        private readonly IMapper _mapper;
        public ShipmentItemService(IShipmentItemRepository shipmentItemRepository, IMapper mapper)
        {
            _shipmentItemRepository = shipmentItemRepository ?? throw new ArgumentNullException(nameof(shipmentItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ShipmentItemDto> AddAsync(CreateShipmentItemDto shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            var model = _mapper.Map<ShipmentItem>(shipmentItem);
            await _shipmentItemRepository.AddAsync(model);

            return _mapper.Map<ShipmentItemDto>(model);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var shipmentItem = await _shipmentItemRepository.GetByIdAsync(id);
            if (shipmentItem == null)
                throw new KeyNotFoundException($"ID {id} ile shipmentItem bulunamadı.");

            await _shipmentItemRepository.DeleteAsync(shipmentItem);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return await _shipmentItemRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByShipmentIdAsync(int shipmentId)
        {
            if (shipmentId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shipmentId));
            }

            return await _shipmentItemRepository.ExistsByShipmentIdAsync(shipmentId);
        }

        public async Task<List<ShipmentItemDto>> GetAllAsync()
        {
            var shipmentItem = await _shipmentItemRepository.GetAllAsync();
            var shipmentItemDtos = _mapper.Map<IEnumerable<ShipmentItemDto>>(shipmentItem);
            return shipmentItemDtos.ToList();
        }

        public async Task<PagedResult<ShipmentItemDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));


            var shipItems = await _shipmentItemRepository.GetAllAsync(pageNumber, pageSize);
            return new PagedResult<ShipmentItemDto>
            {
                Items = _mapper.Map<IEnumerable<ShipmentItemDto>>(shipItems.Items),
                TotalCount = shipItems.TotalCount,
                PageNumber = shipItems.PageNumber,
                PageSize = shipItems.PageSize
            };
        }

        public async Task<ShipmentItemDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var shipmentItem = await _shipmentItemRepository.GetByIdAsync(id);
            if (shipmentItem == null)
            {
                return null;
            }
            var shipmentItemDto = _mapper.Map<ShipmentItemDto>(shipmentItem);

            return shipmentItemDto;
        }

        public async Task<ShipmentItemDto?> GetByShipmentIdAsync(int shipmentId)
        {
            if (shipmentId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shipmentId));
            }

            var shipmentItem = await _shipmentItemRepository.GetByShipmentIdAsync(shipmentId);
            if (shipmentItem == null)
            {
                return null;
            }
            var shipmentItemDto = _mapper.Map<ShipmentItemDto>(shipmentItem);

            return shipmentItemDto;
        }

        public async Task<PagedResult<ShipmentItem>> GetPagedAsync(GridCommand gridCommand)
        {
            var shipmentItems = await _shipmentItemRepository.GetPagedAsync(gridCommand);
            return new PagedResult<ShipmentItem>
            {
                Items = _mapper.Map<IEnumerable<ShipmentItem>>(shipmentItems.Items),
                TotalCount = shipmentItems.TotalCount,
                PageNumber = shipmentItems.PageNumber,
                PageSize = shipmentItems.PageSize
            };
        }

        public async Task<ShipmentItemDto> UpdateAsync(UpdateShipmentItemDto shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            var existingShipmentItem = await _shipmentItemRepository.GetByIdAsync(shipmentItem.Id);
            if (existingShipmentItem == null)
                throw new KeyNotFoundException($"ID {shipmentItem.Id} ile ShipmentItem bulunamadı.");

            _mapper.Map(shipmentItem, existingShipmentItem);
            await _shipmentItemRepository.UpdateAsync(existingShipmentItem);

            return _mapper.Map<ShipmentItemDto>(existingShipmentItem);
        }
    }
}
