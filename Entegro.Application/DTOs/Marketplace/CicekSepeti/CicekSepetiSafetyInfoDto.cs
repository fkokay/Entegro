using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.CicekSepeti
{
    public class CicekSepetiSafetyInfoDto
    {
        [JsonProperty("warningDescription")]
        public string WarningDescription { get; set; }

        [JsonProperty("ceSymbolValueId")]
        public int CeSymbolValueId { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("frontImage")]
        public string FrontImage { get; set; }

        [JsonProperty("backImage")]
        public string BackImage { get; set; }
    }
}
