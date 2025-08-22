using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("ProductIntegration")]
    public class ProductIntegration : BaseEntity
    {
        public int ProductId { get; set; }
        public int IntegrationSystemId { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; }
    }
}
