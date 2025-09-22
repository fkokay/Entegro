namespace Entegro.Web.Models.Setting
{
    public class UpdateSettingModel
    {
        public int Id { get; set; }
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
    }
}
