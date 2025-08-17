namespace Entegro.Application.DTOs.IntegrationSystemLog
{
    public class CreateIntegrationSystemLogDto
    {
        public int Id { get; set; }
        public int IntegrationSystemId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string LogLevel { get; set; } // e.g., "Info", "Warning", "Error"
        public string? Exception { get; set; } // Optional exception details
    }
}
