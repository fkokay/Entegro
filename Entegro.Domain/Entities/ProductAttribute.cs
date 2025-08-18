using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("ProductAttribute")]
    public class ProductAttribute : BaseEntity, IDisplayOrder
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }

        public ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();
    }
}
