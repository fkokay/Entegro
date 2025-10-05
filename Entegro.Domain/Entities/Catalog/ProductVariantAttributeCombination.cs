using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities.Catalog
{
    public class ProductVariantAttributeCombinationMap : IEntityTypeConfiguration<ProductVariantAttributeCombination>
    {
        public void Configure(EntityTypeBuilder<ProductVariantAttributeCombination> builder)
        {
            builder.Property(p => p.Price).HasPrecision(18, 4);
        }
    }

    [Table("ProductVariantAttributeCombination")]
    public class ProductVariantAttributeCombination : BaseEntity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public string? StokCode { get; set; }
        public string? Gtin { get; set; }
        public string? ManufacturerPartNumber { get; set; }
        public decimal? Price { get; set; }
        public int StockQuantity { get; set; }
        public string AssignedMediaFileIds { get; set; } = string.Empty;
        public string RawAttribute { get; set; }

        /// <summary>
        /// Gets the assigned media file identifiers.
        /// </summary>
        public int[] GetAssignedMediaIds()
        {
            if (string.IsNullOrEmpty(AssignedMediaFileIds))
            {
                return [];
            }

            var query =
                from id in AssignedMediaFileIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                let idx = id.ToInt()
                where idx > 0
                select idx;

            return query.Distinct().ToArray();
        }

        /// <summary>
        /// Sets the assigned media file identifiers.
        /// </summary>
        public void SetAssignedMediaIds(int[] ids)
        {
            AssignedMediaFileIds = ids?.Length > 0 ? string.Join(',', ids) : "";
        }
    }
}
