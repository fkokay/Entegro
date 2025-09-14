using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreApiContext
    {
        public string BaseUrl { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
    }
}
