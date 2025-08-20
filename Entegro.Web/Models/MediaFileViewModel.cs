using Entegro.Domain.Entities;

namespace Entegro.Web.Models
{
    public class MediaFileViewModel
    {
        public int Id { get; set; }
        public int? FolderId { get; set; }
        public string Name { get; set; } = null!;
        public string Alt { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Extension { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string MediaType { get; set; } = null!;
        public int Size { get; set; }
        public int PixelSize { get; set; }
        public string? Metadata { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsTransient { get; set; }
        public bool Deleted { get; set; }

        public MediaFolderViewModel? Folder { get; set; }

        public string Url
        {
            get
            {
                if (this.Folder == null)
                {
                    return $"/media/{Id}/{Name}";
                }
                else
                {
                    return $"/media/{Id}/{Folder.Name}/{Name}";
                }
               
            }
        }
    }
}
