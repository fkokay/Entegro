using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.N11
{
    public class N11Attribute
    {
        [JsonPropertyName("attributeId")]
        public int AttributeId { get; set; }

        [JsonPropertyName("attributeName")]
        public string AttributeName { get; set; }

        [JsonPropertyName("attributeValue")]
        public string AttributeValue { get; set; }
    }
}
