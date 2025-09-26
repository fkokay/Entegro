using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.N11
{
    public class N11PackageHistoryDto
    {
        [JsonProperty("createdDate")]
        public object CreatedDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
