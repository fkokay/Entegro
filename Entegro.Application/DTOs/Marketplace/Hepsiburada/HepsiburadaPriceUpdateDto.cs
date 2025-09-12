using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Hepsiburada
{
    public class HepsiburadaPriceUpdateDto
    {
        [JsonProperty("hepsiburadaSku")]
        public string? HepsiburadaSku { get; set; }
        [JsonProperty("merchantSku")]
        public string? MerchantSku { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
