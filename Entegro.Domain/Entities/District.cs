using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    public class District : BaseEntity
    {
        public int TownId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
    }
}
