using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Pazarama
{
    public class PazaramaResponse<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("messageCode")]
        public string MessageCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("userMessage")]
        public string UserMessage { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
