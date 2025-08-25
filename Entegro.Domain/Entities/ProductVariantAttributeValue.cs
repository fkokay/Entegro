using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("ProductVariantAttributeValue")]
    public class ProductVariantAttributeValue : BaseEntity
    {
        public int ProductVariantAttributeId { get; set; }

        private ProductVariantAttribute _productVariantAttribute;
        public ProductVariantAttribute ProductVariantAttribute
        {
            get => _productVariantAttribute ?? LazyLoader.Load(this, ref _productVariantAttribute);
            set => _productVariantAttribute = value;
        }
        public string Name { get; set; }
    }
}
