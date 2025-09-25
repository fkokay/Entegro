using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Customer;

namespace Entegro.Application.DTOs.CustomerAddressMapping
{
    public class CreateCustomerAddressMappingDto
    {
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public AddressDto Address { get; set; }
        public CustomerDto Customer { get; set; }
    }


}
