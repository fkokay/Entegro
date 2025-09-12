using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Hepsiburada
{
    public class HepsiburadaStockUpdateDto
    {
        [JsonProperty("hepsiburadaSku")]
        public string? HepsiburadaSku { get; set; }
        [JsonProperty("merchantSku")]
        public string? MerchantSku { get; set; }
        [JsonProperty("availableStock")]
        public int AvailableStock { get; set; }
        [JsonProperty("maximumPurchasableQuantity")]
        public int? MaximumPurchasableQuantity { get; set; }
    }
}
