using Entegro.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    [Table("City")]
    public class City : BaseEntity
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
        public List<Town> Towns { get; set; }
    }
}
