using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Pazarama
{
    public class PazaramaProductSaleLimitDetailDto
    {
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
