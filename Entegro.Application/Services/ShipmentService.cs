using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Checkout;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;
        public ShipmentService(IShipmentRepository shipmentRepository, IMapper mapper)
        {
            _shipmentRepository = shipmentRepository ?? throw new ArgumentNullException(nameof(shipmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ShipmentDto> AddAsync(CreateShipmentDto shipment)
        {

            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var model = _mapper.Map<Shipment>(shipment);
            await _shipmentRepository.AddAsync(model);

            return _mapper.Map<ShipmentDto>(model);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var shipment = await _shipmentRepository.GetByIdAsync(id);
            if (shipment == null)
                throw new KeyNotFoundException($"ID {id} ile Shipment bulunamadı.");

            await _shipmentRepository.DeleteAsync(shipment);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return await _shipmentRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(orderId));
            }

            return await _shipmentRepository.ExistsByIdAsync(orderId);
        }

        public async Task<bool> ExistsByTrackingNumberAsync(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber))
            {
                throw new ArgumentOutOfRangeException(nameof(trackingNumber));
            }

            return await _shipmentRepository.ExistsByTrackingNumberAsync(trackingNumber);
        }

        public async Task<List<ShipmentDto>> GetAllAsync()
        {
            var shipments = await _shipmentRepository.GetAllAsync();
            var shipmentDtos = _mapper.Map<IEnumerable<ShipmentDto>>(shipments);
            return shipmentDtos.ToList();
        }

        public async Task<PagedResult<ShipmentDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));


            var shipments = await _shipmentRepository.GetAllAsync(pageNumber, pageSize);
            return new PagedResult<ShipmentDto>
            {
                Items = _mapper.Map<IEnumerable<ShipmentDto>>(shipments.Items),
                TotalCount = shipments.TotalCount,
                PageNumber = shipments.PageNumber,
                PageSize = shipments.PageSize
            };
        }

        public async Task<ShipmentDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var shipment = await _shipmentRepository.GetByIdAsync(id);
            if (shipment == null)
            {
                return null;
            }
            var ShipmentDto = _mapper.Map<ShipmentDto>(shipment);

            return ShipmentDto;
        }

        public async Task<ShipmentDto?> GetByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(orderId));
            }

            var shipment = await _shipmentRepository.GetByOrderIdAsync(orderId);
            if (shipment == null)
            {
                return null;
            }
            var ShipmentDto = _mapper.Map<ShipmentDto>(shipment);

            return ShipmentDto;
        }

        public async Task<ShipmentDto?> GetByTrackingNumberAsync(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber))
            {
                throw new ArgumentOutOfRangeException(nameof(trackingNumber));
            }

            var shipment = await _shipmentRepository.GetByTrackingNumberAsync(trackingNumber);
            if (shipment == null)
            {
                return null;
            }
            var ShipmentDto = _mapper.Map<ShipmentDto>(shipment);

            return ShipmentDto;
        }

        public async Task<PagedResult<ShipmentDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var shipment = await _shipmentRepository.GetPagedAsync(gridCommand);

            var items = await shipment.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<ShipmentDto>(x);
                model.CreatedOn = x.CreatedOnUtc.ToLocalTime();
                model.ShippedDate = x.ShippedDateUtc.ToLocalTime();
                model.DeliveryDate = x.DeliveryDateUtc.ToLocalTime();
                return model;
            }).AsyncToList();

            return new PagedResult<ShipmentDto>
            {
                Items = _mapper.Map<IEnumerable<ShipmentDto>>(shipment.Items),
                TotalCount = shipment.TotalCount,
                PageNumber = shipment.PageNumber,
                PageSize = shipment.PageSize
            };
        }

        public async Task<ShipmentDto> UpdateAsync(UpdateShipmentDto shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var existingShipment = await _shipmentRepository.GetByIdAsync(shipment.Id);
            if (existingShipment == null)
                throw new KeyNotFoundException($"ID {shipment.Id} ile shipment bulunamadı.");

            _mapper.Map(shipment, existingShipment);
            await _shipmentRepository.UpdateAsync(existingShipment);

            return _mapper.Map<ShipmentDto>(existingShipment);
        }

        public async Task<ShipmentDto> UpdateByDeliveryDateAsync(int id)
        {
            if (id < 0 == null)
                throw new ArgumentOutOfRangeException(nameof(id));

            var existingShipment = await _shipmentRepository.GetByIdAsync(id);
            if (existingShipment == null)
                throw new KeyNotFoundException($"ID {id} ile shipment bulunamadı.");

            await _shipmentRepository.UpdateByDeliveryDateAsync(existingShipment.Id);
            return _mapper.Map<ShipmentDto>(existingShipment);
        }

        public async Task<ShipmentDto> UpdateByShippedDateAsync(int id)
        {
            if (id < 0 == null)
                throw new ArgumentOutOfRangeException(nameof(id));

            var existingShipment = await _shipmentRepository.GetByIdAsync(id);
            if (existingShipment == null)
                throw new KeyNotFoundException($"ID {id} ile shipment bulunamadı.");

            await _shipmentRepository.UpdateByShippedDateAsync(existingShipment.Id);
            return _mapper.Map<ShipmentDto>(existingShipment);
        }
    }
}
