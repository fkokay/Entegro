namespace Entegro.Web.Models
{
    public class UpdateBrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public int DisplayOrder { get; set; }
    }
}
