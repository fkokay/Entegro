namespace Entegro.Web.Models.Integration
{
    public class IntegrationSystemParameterViewModel
    {
        public int Id { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
