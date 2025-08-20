using Entegro.Application.DTOs.MediaFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Exceptions
{
    public sealed class DuplicateMediaFileException : Exception
    {
        public DuplicateMediaFileException(string message, MediaFileDto dupeFile, string uniquePath) : base(message)
        {
            File = dupeFile;
            UniquePath = uniquePath;
        }

        public MediaFileDto File { get; }
        public string UniquePath { get; }
    }
}
