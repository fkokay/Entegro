using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Checkout
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {

        }
    }
    [Table("Customer")]
    public class Customer : BaseEntity, IAuditable
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Town { get; set; }
        public string? Street { get; set; }
        public string? Address { get; set; }
        public int CustomerType { get; set; } // 0: Individual, 1: Corporate
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<CustomerAddressMapping> CustomerAddressMappings { get; set; } = new HashSet<CustomerAddressMapping>();

    }
}
