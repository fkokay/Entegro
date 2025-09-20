namespace Entegro.Web.Models.Import
{
    public class XmlImportProfileModel
    {
        public string? MediaFileUrl { get; set; }
        public string ProfileName { get; set; } = null!;
        public string? MediaFileType { get; set; }
        public List<string> Paths { get; set; } = new();
        public bool Enable { get; set; } = true;
        public string? Error { get; set; }
        public string? PreviewXml { get; set; }
    }
}
