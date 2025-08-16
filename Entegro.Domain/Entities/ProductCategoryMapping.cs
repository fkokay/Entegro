using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("ProductCategoryMapping")]
    public class ProductCategoryMapping : BaseEntity
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Category Category { get; set; }
        public virtual Product Product { get; set; }
    }
}
