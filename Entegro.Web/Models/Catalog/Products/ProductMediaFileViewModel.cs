using Entegro.Web.Models.Content;

namespace Entegro.Web.Models.Catalog.Products
{
    public class ProductMediaFileViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MediaFileId { get; set; }
        public string? Url { get; set; }
        public int DisplayOrder { get; set; }

        public ProductViewModel Product { get; set; }
        public MediaFileViewModel MediaFile { get; set; }
    }
}
