using Entegro.Domain.Enums;

namespace Entegro.Web.Models.Integration.Common
{
    public class IntegrationSystemViewModel
    {
        public int Id { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public IntegrationSystemType IntegrationSystemType { get; set; }
        public string IntegrationSystemTypeLabelHint { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public List<IntegrationSystemParameterViewModel> IntegrationSystemParameters { get; set; } = new List<IntegrationSystemParameterViewModel>();

    }
}
