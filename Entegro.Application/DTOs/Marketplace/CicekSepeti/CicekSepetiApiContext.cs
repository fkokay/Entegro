using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.CicekSepeti
{
    public class CicekSepetiApiContext
    {
        public string BaseUrl = "https://apis.ciceksepeti.com/api/v1/";
        public string ApiUser { get; set; }
        public string SupplierId { get; set; }
    }
}
