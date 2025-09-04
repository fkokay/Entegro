using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    public class ProductAttributeValueMap : IEntityTypeConfiguration<ProductAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {

        }
    }
    [Table("ProductAttributeValue")]
    public class ProductAttributeValue : BaseEntity, IDisplayOrder
    {
        public int ProductAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }


        private ProductAttribute? _productAttribute;
        public ProductAttribute? ProductAttribute
        {
            get => _productAttribute ?? LazyLoader?.Load(this, ref _productAttribute);
            set => _productAttribute = value;
        }
    }
}
