using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("ProductImageMapping")]
    public class ProductImageMapping : BaseEntity, IDisplayOrder
    {
        public int ProductId { get; set; }
        public string Url { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Product Product { get; set; }
    }
}
