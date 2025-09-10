using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Catalog
{
    public class SpecificationAttributeOptionMap : IEntityTypeConfiguration<SpecificationAttributeOption>
    {
        public void Configure(EntityTypeBuilder<SpecificationAttributeOption> builder)
        {

        }
    }

    [Table("SpecificationAttributeOption")]
    public class SpecificationAttributeOption : BaseEntity, IDisplayOrder
    {
        public int SpecificationAttributeId { get; set; }
        public virtual SpecificationAttribute SpecificationAttribute { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
