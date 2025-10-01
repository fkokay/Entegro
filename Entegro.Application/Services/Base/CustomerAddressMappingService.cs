using Entegro.Application.DTOs.CustomerAddressMapping;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Checkout;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class CustomerAddressMappingService : ICustomerAddressMappingService
    {
        private readonly ICustomerAddressMappingRepository _customerAddressMappingRepository;
        private readonly IMapper _mapper;
        public CustomerAddressMappingService(ICustomerAddressMappingRepository customerAddressMappingRepository, IMapper mapper)
        {
            _customerAddressMappingRepository = customerAddressMappingRepository ?? throw new ArgumentNullException(nameof(customerAddressMappingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CustomerAddressMappingDto> AddAsync(CreateCustomerAddressMappingDto mapping)
        {
            if (await _customerAddressMappingRepository.ExistsAsync(mapping.CustomerId, mapping.AddressId))
                throw new InvalidOperationException("Bu müşteri ve adres eşlemesi zaten mevcut.");

            var entity = _mapper.Map<CustomerAddressMapping>(mapping);
            await _customerAddressMappingRepository.AddAsync(entity);
            var result = await _customerAddressMappingRepository.GetAsync(mapping.CustomerId, mapping.AddressId);
            return _mapper.Map<CustomerAddressMappingDto>(result);
        }

        public async Task DeleteAsync(int customerId, int addressId)
        {
            var entity = await _customerAddressMappingRepository.GetAsync(customerId, addressId);

            if (entity is null)
                throw new KeyNotFoundException("Müşteri-adres eşlemesi bulunamadı.");

            await _customerAddressMappingRepository.DeleteAsync(entity);
        }
        public async Task<bool> ExistsAsync(int customerId, int addressId)
        {
            return await _customerAddressMappingRepository.ExistsAsync(customerId, addressId);
        }
        public async Task<CustomerAddressMappingDto?> GetAsync(int customerId, int addressId)
        {
            var entity = await _customerAddressMappingRepository.GetAsync(customerId, addressId);
            return entity == null ? null : _mapper.Map<CustomerAddressMappingDto>(entity);
        }
        public async Task<IEnumerable<CustomerAddressMappingDto>> GetByCustomerIdAsync(int customerId)
        {
            var entities = await _customerAddressMappingRepository.GetByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<CustomerAddressMappingDto>>(entities);
        }
        public async Task<CustomerAddressMappingDto> UpdateAsync(UpdateCustomerAddressMappingDto mapping)
        {
            var entity = await _customerAddressMappingRepository.GetAsync(mapping.CustomerId, mapping.AddressId);

            if (entity is null)
                throw new KeyNotFoundException("Müşteri-adres eşlemesi bulunamadı.");
            await _customerAddressMappingRepository.UpdateAsync(entity);
            return _mapper.Map<CustomerAddressMappingDto>(entity);
        }
    }
}
