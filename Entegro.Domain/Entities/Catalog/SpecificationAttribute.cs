using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Catalog
{
    public class SpecificationAttributeMap : IEntityTypeConfiguration<SpecificationAttribute>
    {
        public void Configure(EntityTypeBuilder<SpecificationAttribute> builder)
        {

        }
    }
    [Table("SpecificationAttribute")]
    public class SpecificationAttribute : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<SpecificationAttributeOption> SpecificationAttributeOptions { get; set; } = new HashSet<SpecificationAttributeOption>();
    }
}
