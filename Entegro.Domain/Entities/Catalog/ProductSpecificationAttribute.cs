using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Catalog
{
    [Table("Product_SpecificationAttribute_Mapping")]
    public class ProductSpecificationAttribute :BaseEntity
    {
        public int SpecificationAttributeOptionId { get; set; }
        public virtual SpecificationAttributeOption SpecificationAttributeOption { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int DisplayOrder { get; set; }
    }
}
