using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("Town")]
    public class Town : BaseEntity
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }

        public List<District> Districts { get; set; }
    }
}
