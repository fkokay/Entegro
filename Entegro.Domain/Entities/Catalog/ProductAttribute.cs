using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities.Catalog
{
    public class ProductAttributeMap : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {

        }
    }
    [Table("ProductAttribute")]
    public class ProductAttribute : BaseEntity, IDisplayOrder
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public  virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new HashSet<ProductAttributeValue>();

    }
}
