namespace Entegro.Application.DTOs.IntegrationSystem
{
    public class UpdateIntegrationSystemDto
    {
        public int Id { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
