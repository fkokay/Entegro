using Entegro.Web.Models.Content;

namespace Entegro.Web.Models.Catalog.Products
{
    public class ProductMediaFileModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MediaFileId { get; set; }
        public string? Url { get; set; }
        public int DisplayOrder { get; set; }

        public ProductModel Product { get; set; }
        public MediaFileModel MediaFile { get; set; }
    }
}
