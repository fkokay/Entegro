using Entegro.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    [Table("MediaFolder")]
    public class MediaFolder : BaseEntity
    {
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

        public virtual MediaFolder? Parent { get; set; }
        public virtual ICollection<MediaFile>? MediaFiles { get; set; } = new List<MediaFile>();
    }
}
