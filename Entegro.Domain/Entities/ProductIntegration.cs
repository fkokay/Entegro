
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;
namespace Entegro.Domain.Entities
{
    public class ProductIntegrationMap : IEntityTypeConfiguration<ProductIntegration>
    {
        public void Configure(EntityTypeBuilder<ProductIntegration> builder)
        {
            builder.HasIndex(p => new { p.IntegrationSystemId, p.IntegrationCode }).IsUnique();
            builder.Property(p => p.Price).HasPrecision(18, 4);
        }
    }
    [Table("ProductIntegration")]
    public class ProductIntegration : BaseEntity
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public virtual Product Product { get; set; }
        public int IntegrationSystemId { get; set; }
        public virtual IntegrationSystem IntegrationSystem { get; set; }
        public string IntegrationCode { get; set; }
        public string? Custom { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool IsSync { get; set; }
        public bool Active { get; set; }

    }
}
