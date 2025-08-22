using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    [Table("MediaFile")]
    public class MediaFile : BaseEntity, ITransient
    {
        public int? FolderId { get; set; }
        private MediaFolder? _folder;
        public MediaFolder? Folder
        {
            get => _folder ?? LazyLoader?.Load(this, ref _folder);
            set => _folder = value;
        }
        public string Name { get; set; }
        public string Alt { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public string MediaType { get; set; }
        public int Size { get; set; }
        public int PixelSize { get; set; }
        public string? Metadata { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsTransient { get; set; }
        public bool Deleted { get; set; }
        public bool Hidden { get; set; }
        public int Version { get; set; }




    }
}
