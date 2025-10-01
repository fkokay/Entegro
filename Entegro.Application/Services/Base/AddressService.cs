using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Common;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        public AddressService(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AddressDto> AddAsync(CreateAddressDto address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var model = _mapper.Map<Address>(address);
            await _addressRepository.AddAsync(model);

            return _mapper.Map<AddressDto>(model);
        }

        public async Task DeleteAsync(int id)
        {

            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
                throw new KeyNotFoundException($"ID {id} ile Adres bulunamadı.");

            await _addressRepository.DeleteAsync(address);
        }

        public async Task<List<AddressDto>> GetAllAsync()
        {
            var addresss = await _addressRepository.GetAllAsync();
            var addressDtos = _mapper.Map<List<AddressDto>>(addresss);
            return addressDtos;
        }

        public async Task<AddressDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
            {
                return null;
            }
            var addressDto = _mapper.Map<AddressDto>(address);

            return addressDto;
        }

        public async Task<PagedResult<AddressDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            var addresss = await _addressRepository.GetAllAsync();
            var addressDtos = _mapper.Map<PagedResult<AddressDto>>(addresss);
            return addressDtos;
        }

        public async Task<PagedResult<AddressDto>> GetPagedAsync(GridCommand gridCommand, int customerId)
        {
            var addresses = await _addressRepository.GetPagedAsync(gridCommand, customerId);

            var items = await addresses.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<AddressDto>(x);
                model.CreatedOn = x.CreatedOnUtc.ToLocalTime();
                model.UpdatedOn = x.UpdatedOnUtc.ToLocalTime();
                return model;
            }).AsyncToList();
            return new PagedResult<AddressDto>
            {
                Items = items,
                TotalCount = addresses.TotalCount,
                PageNumber = addresses.PageNumber,
                PageSize = addresses.PageSize
            };
        }

        public async Task<AddressDto> UpdateAsync(UpdateAddressDto address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var existingAddress = await _addressRepository.GetByIdAsync(address.Id);
            if (existingAddress == null)
                throw new KeyNotFoundException($"ID {address.Id} ile Address bulunamadı.");

            _mapper.Map(address, existingAddress);
            await _addressRepository.UpdateAsync(existingAddress);

            return _mapper.Map<AddressDto>(existingAddress);
        }
    }
}

