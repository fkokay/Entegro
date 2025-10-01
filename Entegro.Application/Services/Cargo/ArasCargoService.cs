using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Services.Cargo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Cargo
{
    public class ArasCargoService : IArasCargoService
    {
        public Task CancelDispatch(string integrationCode)
        {
            throw new NotImplementedException();
        }

        public Task GetBarcode(string integrationCode)
        {
            throw new NotImplementedException();
        }

        public Task GetCargo(string queryType, string integrationCode)
        {
            throw new NotImplementedException();
        }

        public Task SendCargo(ShipmentDto shipmentDto, bool isDoorPayment)
        {
            throw new NotImplementedException();
        }
    }
}
