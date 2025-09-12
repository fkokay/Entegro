using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Pazarama
{
    public class PazaramaApiContext
    {
        public string BaseUrlToken = "https://isortagimgiris.pazarama.com/";
        public string BaseUrl = "https://isortagimapi.pazarama.com/";
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SupplierId { get; set; }
    }
}
