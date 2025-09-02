using Entegro.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    [Table("SpecificationAttribute")]
    public class SpecificationAttribute : BaseEntity
    {
        public string Name { get; set; }
    }
}
