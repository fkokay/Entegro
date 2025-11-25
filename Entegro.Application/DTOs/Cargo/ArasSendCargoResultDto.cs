namespace Entegro.Application.DTOs.Cargo
{
    public class ArasSendCargoResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? PrintData { get; set; }
        public string? TrackingNumber { get; set; }
    }
}
