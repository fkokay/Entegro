using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Order;
using Entegro.Application.Mappings.Marketplace.Trendyol;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.N11
{
    public static class N11ShipmentPackageMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderDto? ToDto(N11ShipmentPackageDto n11ShipmentPackage)
        {
            if (n11ShipmentPackage == null)
            {
                return null;
            }

            OrderDto order = new OrderDto();
  
            


            return order;
        }

        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<N11ShipmentPackageDto> n11ShipmentPackages)
        {
            if (n11ShipmentPackages == null)
                yield break;

            foreach (var n11ShipmentPackage in n11ShipmentPackages)
            {
                var dto = ToDto(n11ShipmentPackage);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
