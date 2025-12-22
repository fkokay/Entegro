using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.ReturnRequestItem;
using Entegro.Domain.Entities.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IReturnRequestItemService
    {
        Task<ReturnRequestItemDto?> GetByIdAsync(int id);
        Task<List<ReturnRequestItemDto>> GetAllWithIntegrationSkuAsync(string integrationSku);
        Task<ReturnRequestItemDto> UpdateAsync(UpdateReturnRequestItemDto updateReturnRequestItem);
    }
}
