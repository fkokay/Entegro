using Entegro.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    [Table("IntegrationSystemParameter")]
    public class IntegrationSystemParameter : BaseEntity
    {
        public int IntegrationSystemId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public virtual IntegrationSystem IntegrationSystem { get; set; }
    }
}
