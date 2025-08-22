using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    [Table("ProductAttributeValue")]
    public class ProductAttributeValue : BaseEntity, IDisplayOrder
    {
        public int ProductAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }


        private ProductAttribute? _productAttribute;
        public ProductAttribute? ProductAttribute
        {
            get => _productAttribute ?? LazyLoader?.Load(this, ref _productAttribute);
            set => _productAttribute = value;
        }
    }
}
