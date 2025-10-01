using Entegro.Application.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Cargo
{
    public interface IArasCargoService
    {
        Task SendCargo(ShipmentDto shipmentDto, bool isDoorPayment);
        Task GetBarcode(string integrationCode);
        Task CancelDispatch(string integrationCode);
        Task GetCargo(string queryType,string integrationCode);
    }
}
