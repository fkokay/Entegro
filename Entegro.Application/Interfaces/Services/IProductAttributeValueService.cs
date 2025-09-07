using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttributeValue;
using System;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductAttributeValueService
    {
        Task<ProductAttributeValueDto?> GetByIdAsync(int id);
        Task<ProductAttributeValueDto?> GetByNameAsync(string name);
        Task<ProductAttributeValueDto?> GetByNameOrAttributeIdAsync(string name,int attributeId);
        Task<List<ProductAttributeValueDto>> GetAllAsync();
        Task<PagedResult<ProductAttributeValueDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PagedResult<ProductAttributeValueDto>> GetPagedAsync(GridCommand gridCommand);
        Task<ProductAttributeValueDto> AddAsync(CreateProductAttributeValueDto productAttributeValue);
        Task<ProductAttributeValueDto> UpdateAsync(UpdateProductAttributeValueDto productAttributeValue);
        Task DeleteAsync(int productAttributeId);
    }
}
