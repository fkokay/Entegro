using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Domain.Entities.Checkout;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Entegro.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly EntegroDbContext _context;

        public CustomerRepository(EntegroDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(Customer customer)
        {
            customer.CreatedOnUtc = DateTime.UtcNow;
            customer.UpdatedOnUtc = DateTime.UtcNow;
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Customers.Where(m=> m.Email == email).AnyAsync();
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<Customer>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Customers.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Customer>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(o => o.Email == email);
        }

        public async Task UpdateAsync(Customer customer)
        {
            customer.UpdatedOnUtc = DateTime.UtcNow;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Application.DTOs.Common.PagedResult<Customer>> GetPagedAsync(GridCommand gridCommand)
        {
            var query = _context.Customers.AsNoTracking();

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b => b.Name.Contains(gridCommand.Search.Value)).AsQueryable();
                }
            }

            if (gridCommand.Order.Any())
            {
                foreach (var item in gridCommand.Order)
                {
                    query = query.OrderBy($"{gridCommand.Columns[item.Column].Data} {(item.Dir ?? "asc")}");
                }
            }
            else
            {
                query = query.OrderBy(b => b.Id);
            }

            var totalCount = await query.CountAsync();
            var items = await query
            .Skip(gridCommand.Start)
            .Take(gridCommand.Length)
            .ToListAsync();

            return new Application.DTOs.Common.PagedResult<Customer>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start + 1,
                PageSize = gridCommand.Length
            };
        }
    }
}
