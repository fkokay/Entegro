using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
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
    }
}
