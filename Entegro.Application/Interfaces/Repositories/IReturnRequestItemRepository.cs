using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IReturnRequestItemRepository
    {
        Task<ReturnRequestItem?> GetByIdAsync(int id);
        Task<List<ReturnRequestItem>> GetAllIntegrationSkuWithAsync(string integrationSku);
        Task UpdateAsync(ReturnRequestItem returnRequestItem);
    }
}
