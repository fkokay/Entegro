namespace Entegro.Web.Models.Catalog.RelatedProducts
{
    public class RelatedProductModel
    {
        public int Id { get; set; }
        public int ProductId1 { get; set; }
        public int ProductId2 { get; set; }
        public int DisplayOrder { get; set; }
    }
}
