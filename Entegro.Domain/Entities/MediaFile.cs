using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("MediaFile")]
    public class MediaFile : BaseEntity, ITransient
    {
        public int? FolderId { get; set; }
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

        public virtual MediaFolder? Folder { get; set; }
    }
}
