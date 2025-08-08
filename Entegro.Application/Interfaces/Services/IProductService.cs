using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(int productId);
        Task<bool> ExistsByNameAsync(string productName);
        Task<bool> ExistsByCodeAsync(string productCode);
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<PagedResult<ProductDto>> GetProductsAsync(int pageNumber, int pageSize);
        Task<int> CreateProductAsync(CreateProductDto createProduct);
        Task<bool> UpdateProductAsync(UpdateProductDto updateProduct);
        Task<bool> DeleteProductAsync(int productId);
    }
}
