using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("ProductAttributeValue")]
    public class ProductAttributeValue : BaseEntity, IDisplayOrder
    {
        public int ProductAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }

        public ProductAttribute ProductAttribute { get; set; }
    }
}
