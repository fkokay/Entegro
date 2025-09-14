using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Commerce
{
    public interface ISmartstoreService
    {
        Task<IEnumerable<SmartstoreProductDto>> GetProductsAsync(SmartstoreApiContext context,int pageSize = 50);
        Task<IEnumerable<SmartstoreCategoryDto>> GetCategoriesAsync(SmartstoreApiContext context);
        Task<IEnumerable<SmartstoreManufacturerDto>> GetManufacturersAsync(SmartstoreApiContext context);
        Task<SmartstoreManufacturerDto?> GetManufacturerAsync(SmartstoreApiContext context, int id);
        Task<IEnumerable<SmartstoreOrderDto>> GetOrdersAsync(SmartstoreApiContext context, int pageSize = 50);
    }
}
