using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.CicekSepeti
{
    public class CicekSepetiOrderResponse
    {
        [JsonProperty("orderListCount")]
        public int OrderListCount { get; set; }

        [JsonProperty("pageCount")]
        public int PageCount { get; set; }

        [JsonProperty("supplierOrderListWithBranch")]
        public List<CicekSepetiOrderDto> SupplierOrderListWithBranch { get; set; } = new List<CicekSepetiOrderDto>();
    }
}
