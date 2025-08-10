using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Customer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreCustomerMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static CustomerDto? ToDto(SmartstoreCustomerDto smartstoreCustomer)
        {
            try
            {
                if (smartstoreCustomer == null)
                {
                    return null;
                }

                CustomerDto customer = new CustomerDto();
                customer.Name = smartstoreCustomer.FullName;
                customer.Email = smartstoreCustomer.Email;
                customer.CreatedOn = DateTime.Now;
                customer.UpdatedOn = DateTime.Now;

                return customer;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Customer mapping sırasında hata oluştu. CustomerId: {CustomerId}", smartstoreCustomer.Id);
                return null;
            }
        }

        public static IEnumerable<CustomerDto> ToDtoList(IEnumerable<SmartstoreCustomerDto> customers)
        {
            if (customers == null)
                yield break;

            foreach (var customer in customers)
            {
                var dto = ToDto(customer);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
