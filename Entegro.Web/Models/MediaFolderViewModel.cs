namespace Entegro.Web.Models
{
    public class MediaFolderViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string TreePath { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public bool CanDetectTracks { get; set; }
        public string? Metadata { get; set; }
        public int FilesCount { get; set; }
        public string Discriminator { get; set; } = null!;
        public string ResKey { get; set; } = null!;
        public bool IncludePath { get; set; }
        public int? Order { get; set; }
    }
}
