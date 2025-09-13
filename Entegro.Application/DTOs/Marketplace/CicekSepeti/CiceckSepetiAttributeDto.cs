using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.CicekSepeti
{
    public class CiceckSepetiAttributeDto
    {
        [JsonProperty("parentId")]
        public int ParentId { get; set; }

        [JsonProperty("parentName")]
        public string ParentName { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("textLength")]
        public int TextLength { get; set; }
    }
}
