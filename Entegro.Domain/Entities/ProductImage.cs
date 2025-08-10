using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    public class ProductImage : BaseEntity, IDisplayOrder
    {
        public int ProductId { get; set; }
        public string Url { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Product Product { get; set; }
    }
}
