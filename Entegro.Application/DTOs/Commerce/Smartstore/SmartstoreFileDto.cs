using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreFileDto
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
    }
}
