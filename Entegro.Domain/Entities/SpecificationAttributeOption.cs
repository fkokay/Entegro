using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
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
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
