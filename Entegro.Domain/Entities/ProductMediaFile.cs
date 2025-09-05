
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    public class ProductMediaFileMap : IEntityTypeConfiguration<ProductMediaFile>
    {
        public void Configure(EntityTypeBuilder<ProductMediaFile> builder)
        {

        }
    }
    [Table("ProductMediaFile")]
    public class ProductMediaFile : BaseEntity, IDisplayOrder
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int MediaFileId { get; set; }
        public virtual MediaFile MediaFile { get; set; }
        public int DisplayOrder { get; set; }
    }
}
