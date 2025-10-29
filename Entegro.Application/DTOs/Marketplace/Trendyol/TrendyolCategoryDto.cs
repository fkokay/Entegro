namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public List<TrendyolCategoryDto> SubCategories { get; set; } = new();
    }

    public class TrendyolCategoryDto2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<TrendyolCategoryAttributeDto> CategoryAttributes { get; set; } = new();
    }
}
