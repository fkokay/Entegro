using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    [Table("City")]
    public class City : BaseEntity
    {
        public int CountryId { get; set; }
        private Country? _country;
        public Country? Country
        {
            get => _country ?? LazyLoader?.Load(this, ref _country);
            set => _country = value;
        }
        public string Name { get; set; }
        public bool Published { get; set; }

        private ICollection<Town> _towns;
        public ICollection<Town> Towns
        {
            get => LazyLoader?.Load(this, ref _towns) ?? (_towns ??= new HashSet<Town>());
            set => _towns = value;
        }
    }
}
