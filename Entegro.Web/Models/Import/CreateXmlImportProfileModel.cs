namespace Entegro.Web.Models.Import
{
    public class CreateXmlImportProfileModel
    {
        public string? MediaFileUrl { get; set; }
        public string ProfileName { get; set; } = null!;
        public string? MediaFileType { get; set; }
        public bool Enable { get; set; } = true;
        public CreateXmlProductImportProfileModel CreateXmlProduct { get; set; } = new();
    }
}
