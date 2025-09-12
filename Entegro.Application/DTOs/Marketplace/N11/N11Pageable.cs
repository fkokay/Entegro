using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.N11
{
    public class N11Pageable
    {
        [JsonPropertyName("sort")]
        public object Sort { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("paged")]
        public bool Paged { get; set; }

        [JsonPropertyName("unpaged")]
        public bool Unpaged { get; set; }
    }
}
