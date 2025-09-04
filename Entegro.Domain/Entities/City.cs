using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    public class CityMap : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {

        }
    }
    [Table("City")]
    public class City : BaseEntity
    {
        public int CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
        public virtual ICollection<Town> Towns { get; set; }
    }
}
