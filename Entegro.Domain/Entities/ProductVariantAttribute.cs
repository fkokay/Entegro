
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Entegro.Domain.Entities
{
    public class ProductVariantAttributeMap : IEntityTypeConfiguration<ProductVariantAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductVariantAttribute> builder)
        {

        }
    }
    [Table("Product_ProductAttribute_Mapping")]
    [Index(nameof(AttributeControlTypeId), Name = "IX_AttributeControlTypeId")]
    [Index(nameof(ProductId), nameof(DisplayOrder), Name = "IX_Product_ProductAttribute_Mapping_ProductId_DisplayOrder")]
    public class ProductVariantAttribute : BaseEntity, IDisplayOrder
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int ProductAttributeId { get; set; }
        public virtual ProductAttribute ProductAttribute { get; set; }
        public bool IsRequried { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int DisplayOrder {get;set; }

        public virtual ICollection<ProductVariantAttributeValue> ProductVariantAttributeValues{get;set;} = new HashSet<ProductVariantAttributeValue>();

    }
}
