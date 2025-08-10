using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<List<Brand>> GetAllAsync();
        Task<PagedResult<Brand>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Brand brand);
    }
}
