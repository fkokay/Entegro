using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Runtime.Serialization;

namespace Entegro.Domain.Entities
{
    [Table("Product_ProductAttribute_Mapping")]
    [Index(nameof(AttributeControlTypeId), Name = "IX_AttributeControlTypeId")]
    [Index(nameof(ProductId), nameof(DisplayOrder), Name = "IX_Product_ProductAttribute_Mapping_ProductId_DisplayOrder")]
    public class ProductVariantAttribute : BaseEntity, IDisplayOrder
    {
        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public bool IsRequried { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int DisplayOrder {get;set; }


        private Product _product;
        [IgnoreDataMember]
        public Product Product
        {
            get => _product ?? LazyLoader.Load(this, ref _product);
            set => _product = value;
        }

        private ProductAttribute _productAttribute;
        /// <summary>
        /// Gets or sets the product attribute.
        /// </summary>
        public ProductAttribute ProductAttribute
        {
            get => _productAttribute ?? LazyLoader.Load(this, ref _productAttribute);
            set => _productAttribute = value;
        }
    }
}
