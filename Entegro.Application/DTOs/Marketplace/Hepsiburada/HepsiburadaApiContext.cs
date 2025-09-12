using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Hepsiburada
{
    public class HepsiburadaApiContext
    {
        public string BaseUrl = "https://listing-external-sit.hepsiburada.com/";
        public string MerchantId { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
        public string UserAgent { get; set; }
    }
}
