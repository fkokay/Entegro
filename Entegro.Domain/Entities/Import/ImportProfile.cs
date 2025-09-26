using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Import
{
    public class ImportProfileMap : IEntityTypeConfiguration<ImportProfile>
    {
        public void Configure(EntityTypeBuilder<ImportProfile> builder)
        {

        }
    }

    [Table("ImportProfiles")]
    public class ImportProfile : BaseEntity
    {
        public string ProfileName { get; set; } = null!;
        public string? ColumnMapping { get; set; }
        public string? MediaFileType { get; set; }
        public string? MediaFileUrl { get; set; }
        public int? MediaFileId { get; set; }

        public bool? ApplyPriceAdjustment { get; set; }
        public decimal? PriceAdjustmentAmount { get; set; }
        public decimal? OptionalExtraAmount { get; set; }
        public string? PriceAdjustmentType { get; set; }
        public bool Enable { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
