namespace Entegro.Web.Models
{
    public class UpdateProductImageViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; }
        public int DisplayOrder { get; set; }
    }
}
