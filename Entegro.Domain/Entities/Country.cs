using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    [Table("Country")]
    public class Country : BaseEntity
    {
        public string Name { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }

        public ICollection<City> _cities;
        public ICollection<City> Cities
        {
            get => LazyLoader?.Load(this, ref _cities) ?? (_cities ??= new HashSet<City>());
            set => _cities = value;
        }
    }
}
