namespace Entegro.Application.DTOs.ImportProfile
{
    public class ImportProfileDto
    {
        public int Id { get; set; }
        public string ProfileName { get; set; } = null!;
        public string? ColumnMapping { get; set; }
        public string? MediaFileType { get; set; }
        public string? MediaFileUrl { get; set; }
        public int? MediaFileId { get; set; }
        public string? PlatformStoreMapping { get; set; }
        public bool? ApplyPriceAdjustment { get; set; }
        public decimal? PriceAdjustmentAmount { get; set; }
        public decimal? OptionalExtraAmount { get; set; }
        public string? PriceAdjustmentType { get; set; }
        public bool Enable { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? TaskId { get; set; }
    }
}
