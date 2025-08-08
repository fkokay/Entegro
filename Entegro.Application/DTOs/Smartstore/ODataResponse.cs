using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Smartstore
{
    public class ODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; }
        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }
    }
}
