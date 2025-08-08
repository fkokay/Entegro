using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.Smartstore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Commerce
{
    public interface ISmartstoreService
    {
        Task<IEnumerable<SmartstoreProductDto>> GetProductsAsync(int pageSize = 50);
        Task<IEnumerable<SmartstoreCategoryDto>> GetCategoriesAsync();
        Task<IEnumerable<SmartstoreManufacturerDto>> GetManufacturersAsync();
        Task<IEnumerable<SmartstoreOrderDto>> GetOrdersAsync(int pageSize = 50);
    }
}
