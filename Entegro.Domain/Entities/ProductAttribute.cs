using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    [Table("ProductAttribute")]
    public class ProductAttribute : BaseEntity, IDisplayOrder
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }

        private ICollection<ProductAttributeValue> _productAttributeValues;
        public ICollection<ProductAttributeValue> ProductAttributeValues
        {
            get => LazyLoader?.Load(this, ref _productAttributeValues) ?? (_productAttributeValues ??= new HashSet<ProductAttributeValue>());
            set => _productAttributeValues = value;
        }
    }
}
