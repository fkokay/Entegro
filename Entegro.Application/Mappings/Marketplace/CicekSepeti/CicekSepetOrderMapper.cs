using Entegro.Application.DTOs.Marketplace.CicekSepeti;
using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Order;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.CicekSepeti
{
    public static class CicekSepetOrderMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderDto? ToDto(CicekSepetiOrderDto cicekSepetiOrder)
        {
            if (cicekSepetiOrder == null)
            {
                return null;
            }

            OrderDto order = new OrderDto();



            return order;
        }

        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<CicekSepetiOrderDto> cicekSepetiOrders)
        {
            if (cicekSepetiOrders == null)
                yield break;

            foreach (var cicekSepetiOrder in cicekSepetiOrders)
            {
                var dto = ToDto(cicekSepetiOrder);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
