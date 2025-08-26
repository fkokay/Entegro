using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreFileItemInfoDto
    {
        public int Id { get; set; }
        public string Directory { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public string ThumbUrl { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long Length { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? FolderId { get; set; }
        public string MimeType { get; set; }
        public string MediaType { get; set; }
        public bool IsTransient { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string Alt { get; set; }
        public string TitleAttribute { get; set; }
        public string AdminComment { get; set; }
    }
}
