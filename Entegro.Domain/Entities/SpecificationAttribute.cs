using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
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
    }
}
