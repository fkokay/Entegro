using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    [Table("Town")]
    public class Town : BaseEntity
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }



        public ICollection<District> _districts;

        public ICollection<District> Districts
        {
            get => LazyLoader?.Load(this, ref _districts) ?? (_districts ??= new HashSet<District>());
            set => _districts = value;
        }
    }
}
