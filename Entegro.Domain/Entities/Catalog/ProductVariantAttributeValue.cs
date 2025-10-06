using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Catalog
{
    public class ProductVariantAttributeValueMap : IEntityTypeConfiguration<ProductVariantAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductVariantAttributeValue> builder)
        {

        }
    }
    [Table("ProductVariantAttributeValue")]
    public class ProductVariantAttributeValue : BaseEntity
    {
        public int ProductVariantAttributeId { get; set; }
        public virtual ProductVariantAttribute ProductVariantAttribute { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
