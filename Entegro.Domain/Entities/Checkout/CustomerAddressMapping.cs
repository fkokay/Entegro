using Entegro.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Checkout
{
    public class CustomerAddressMappingMap : IEntityTypeConfiguration<CustomerAddressMapping>
    {
        public void Configure(EntityTypeBuilder<CustomerAddressMapping> builder)
        {
            builder.ToTable("CustomerAddressMapping");

            builder.HasKey(cam => new { cam.CustomerId, cam.AddressId });

            builder
                .HasOne(cam => cam.Customer)
                .WithMany(c => c.CustomerAddressMappings)
                .HasForeignKey(cam => cam.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(cam => cam.Address)
                .WithMany()
                .HasForeignKey(cam => cam.AddressId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }


    [Table("CustomerAddressMapping")]
    public class CustomerAddressMapping : BaseEntity
    {
        public int CustomerId { get; set; }
        public int AddressId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Address Address { get; set; }
    }
}
