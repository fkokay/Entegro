using Entegro.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Entegro.Domain.Entities
{
    [Table("ProductMediaFile")]
    public class ProductMediaFile : BaseEntity, IDisplayOrder
    {
        public int ProductId { get; set; }
        private Product _product;
        public Product Product
        {
            get => _product ?? LazyLoader.Load(this, ref _product);
            set => _product = value;
        }

        public int MediaFileId { get; set; }

        private MediaFile _mediaFile;
        public MediaFile MediaFile
        {
            get => _mediaFile ?? LazyLoader.Load(this, ref _mediaFile);
            set => _mediaFile = value;
        }
        public int DisplayOrder { get; set; }
    }
}
