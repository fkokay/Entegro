using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("MediaFolder")]
    public class MediaFolder : BaseEntity
    {
        public int? ParentId { get; set; }
        public string TreePath { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool CanDetectTracks { get; set; }
        public string? Metadata { get; set; }
        public int FilesCount { get; set; }
        public string Discriminator { get; set; }
        public string ResKey { get; set; }
        public bool IncludePath { get; set; }
        public int? Order { get; set; }
    }
}
