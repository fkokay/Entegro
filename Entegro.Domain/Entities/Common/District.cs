using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Common
{
    public class DistrictMap : IEntityTypeConfiguration<District>
    {
        public void Configure(EntityTypeBuilder<District> builder)
        {

        }
    }
    [Table("District")]
    public class District : BaseEntity
    {
        public int TownId { get; set; }
        public virtual Town? Town { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
    }
}
