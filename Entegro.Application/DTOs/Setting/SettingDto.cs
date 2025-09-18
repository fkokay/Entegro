namespace Entegro.Application.DTOs.Setting
{
    public class SettingDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
    }
}
