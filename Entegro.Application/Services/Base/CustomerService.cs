using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Domain.Entities.Checkout;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CustomerDto> AddAsync(CreateCustomerDto createCustomer)
        {
            var customer = _mapper.Map<Customer>(createCustomer);
            await _customerRepository.AddAsync(customer);

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> DeleteAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer is null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }
            await _customerRepository.DeleteAsync(customer);
            return true;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _customerRepository.ExistsByEmailAsync(email);
        }

        public async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
        {
            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer is null)
            {
                return null;
            }

            var customerDto = _mapper.Map<CustomerDto>(customer);
            return customerDto;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer is null)
            {
                return null;
            }

            var customerDto = _mapper.Map<CustomerDto>(customer);
            return customerDto;
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return customerDtos;
        }

        public async Task<PagedResult<CustomerDto>> GetCustomersAsync(int pageNumber, int pageSize)
        {
            var customers = await _customerRepository.GetAllAsync(pageNumber, pageSize);
            var customerDtos = _mapper.Map<PagedResult<CustomerDto>>(customers);
            return customerDtos;
        }

        public async Task<PagedResult<CustomerDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var customers = await _customerRepository.GetPagedAsync(gridCommand);
            var items = await customers.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<CustomerDto>(x);
                model.CreatedOn = x.CreatedOnUtc.ToLocalTime();
                model.UpdatedOn = x.UpdatedOnUtc.ToLocalTime();
                return model;
            }).AsyncToList();

            return new PagedResult<CustomerDto>
            {
                Items = items,
                TotalCount = customers.TotalCount,
                PageNumber = customers.PageNumber,
                PageSize = customers.PageSize
            };
        }

        public async Task<bool> UpdateAsync(UpdateCustomerDto updateCustomer)
        {
            await _customerRepository.UpdateAsync(_mapper.Map<Customer>(updateCustomer));
            return true;
        }
    }
}
