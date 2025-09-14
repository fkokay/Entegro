using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Order;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.Hepsiburada
{
    public static class HepsiburadaShipmentPackageMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderDto? ToDto(HepsiburadaShipmentPackageDto hepsiburadaShipmentPackage)
        {
            if (hepsiburadaShipmentPackage == null)
            {
                return null;
            }

            OrderDto order = new OrderDto();



            return order;
        }

        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<HepsiburadaShipmentPackageDto> hepsiburadaShipmentPackages)
        {
            if (hepsiburadaShipmentPackages == null)
                yield break;

            foreach (var hepsiburadaShipmentPackage in hepsiburadaShipmentPackages)
            {
                var dto = ToDto(hepsiburadaShipmentPackage);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
