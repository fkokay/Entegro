using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    public class City :BaseEntity
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }

        public List<Town> Towns { get; set; }
    }
}
