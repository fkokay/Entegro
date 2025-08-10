using Entegro.ERP.Abstractions.DTOs;
using Entegro.ERP.Abstractions.Interfaces;
using Entegro.ERP.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.ERP.Application.Services
{
    public class ProductService : IProductService   
    {
        private readonly IErpProductReader _erpProductReader;

        public ProductService(IErpProductReader erpProductReader)
        {
            _erpProductReader = erpProductReader;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(int page, int pageSize)
        {
            return await _erpProductReader.GetProductsAsync(page, pageSize);
        }
    }
}
