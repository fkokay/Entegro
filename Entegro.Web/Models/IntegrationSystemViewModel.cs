using Entegro.Application.DTOs.IntegrationSystemParameter;

namespace Entegro.Web.Models
{
    public class IntegrationSystemViewModel
    {
        public int Id { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public IntegrationSystemParameterDto IntegrationSystemParameter { get; set; }
    }
}
