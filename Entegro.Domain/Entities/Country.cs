using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    public class CountryMap : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {

        }
    }
    [Table("Country")]
    public class Country : BaseEntity
    {
        public string Name { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }

        private ICollection<City> _cities;
        public ICollection<City> Cities
        {
            get => LazyLoader?.Load(this, ref _cities) ?? (_cities ??= new HashSet<City>());
            set => _cities = value;
        }
    }
}
