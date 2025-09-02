using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("SpecificationAttribute")]
    public class SpecificationAttribute : BaseEntity
    {
        public string Name { get; set; }
    }
}
