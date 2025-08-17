namespace Entegro.Application.DTOs.IntegrationSystemParameter
{
    public class CreateIntegrationSystemParameterDto
    {
        public int Id { get; set; }
        public int IntegrationSystemId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
