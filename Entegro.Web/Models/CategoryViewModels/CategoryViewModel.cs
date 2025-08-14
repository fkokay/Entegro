namespace Entegro.Web.Models.CategoryViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string TreePath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
