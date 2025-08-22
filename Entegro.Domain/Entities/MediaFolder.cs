using Entegro.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Entegro.Domain.Entities
{
    [Table("MediaFolder")]
    public class MediaFolder : BaseEntity
    {
        public int? ParentId { get; set; }
        private MediaFolder? _parent;
        public MediaFolder? Parent
        {
            get => _parent ?? LazyLoader?.Load(this, ref _parent);
            set => _parent = value;
        }
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

        private ICollection<MediaFile> _mediaFiles;
        public ICollection<MediaFile> MediaFiles
        {
            get => LazyLoader?.Load(this, ref _mediaFiles) ?? (_mediaFiles ??= new HashSet<IntegrationSystemLog>());
            set => _mediaFiles = value;
        }
    }
}
