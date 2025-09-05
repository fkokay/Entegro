using Entegro.Application.DTOs.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Marketplace
{
    public interface IMarketplaceBrandReader<TContext>
    {
        Task<IEnumerable<BrandDto>> GetBrandsAsync(TContext context);
    }
}
