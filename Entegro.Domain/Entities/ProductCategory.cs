using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Entegro.Domain.Entities
{
    [Table("Product_Category_Mapping")]
    [Index(nameof(CategoryId), Name = "IX_CategoryId")]
    [Index(nameof(CategoryId), nameof(ProductId), Name = "IX_PCM_Product_and_Category")]
    public class ProductCategory : BaseEntity
    {
        public int CategoryId { get; set; }
        private Category _category;
        public Category Category
        {
            get => _category ?? LazyLoader.Load(this, ref _category);
            set => _category = value;
        }
        public int ProductId { get; set; }
        private Product _product;
        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        public Product Product
        {
            get => _product ?? LazyLoader.Load(this, ref _product);
            set => _product = value;
        }
        public int DisplayOrder { get; set; }

    }
}
