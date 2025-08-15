namespace Entegro.Web.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string TreePath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

    }
}
