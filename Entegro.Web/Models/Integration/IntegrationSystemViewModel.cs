using Entegro.Domain.Enums;

namespace Entegro.Web.Models.Integration
{
    public class IntegrationSystemViewModel
    {
        public int Id { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public IntegrationSystemType IntegrationSystemType { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public IntegrationSystemParameterViewModel IntegrationSystemParameter { get; set; }
        public string? IntegrationSystemTypeName { get; set; }

    }
}
