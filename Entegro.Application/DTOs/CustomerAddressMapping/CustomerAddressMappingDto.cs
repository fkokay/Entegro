using Entegro.Application.DTOs.Address;

namespace Entegro.Application.DTOs.CustomerAddressMapping
{
    public class CustomerAddressMappingDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public AddressDto Address { get; set; }
    }
}
