
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    public class TownMap : IEntityTypeConfiguration<Town>
    {
        public void Configure(EntityTypeBuilder<Town> builder)
        {

        }
    }
    [Table("Town")]
    public class Town : BaseEntity
    {
        public int CityId { get; set; }
        public virtual City City { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
        public virtual ICollection<District> Districts { get; set; } = new HashSet<District>();
    }
}
