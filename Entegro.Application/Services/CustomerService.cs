using AutoMapper;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
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
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public CustomerService(ICustomerRepository customerRepository,IMapper mapper)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateCustomerAsync(CreateCustomerDto createCustomer)
        {
            var customer = _mapper.Map<Customer>(createCustomer);
            await _customerRepository.AddAsync(customer);

            return customer.Id;
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }
            await _customerRepository.DeleteAsync(customer);
            return true;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
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

        public async Task<bool> UpdateCustomerAsync(UpdateCustomerDto updateCustomer)
        {
            await _customerRepository.UpdateAsync(_mapper.Map<Customer>(updateCustomer));
            return true;
        }
    }
}
