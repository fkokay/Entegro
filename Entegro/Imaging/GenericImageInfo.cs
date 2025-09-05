using System.Drawing;

namespace Entegro.Imaging
{
    public sealed class GenericImageInfo : IImageInfo
    {
        public GenericImageInfo()
        {
        }

        public GenericImageInfo(Size size, IImageFormat format)
        {
            Width = size.Width;
            Height = size.Height;
            Format = format;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public byte BitDepth { get; set; } = 24;

        public IImageFormat Format { get; set; }

        public IEnumerable<ImageMetadataEntry> GetMetadata()
            => Enumerable.Empty<ImageMetadataEntry>();
    }
}
