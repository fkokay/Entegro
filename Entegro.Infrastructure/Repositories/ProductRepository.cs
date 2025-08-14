using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Common;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EntegroContext _context;

        public ProductRepository(EntegroContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Product product)
        {
            product.CreatedOn = DateTime.Now;
            product.UpdatedOn = DateTime.Now;
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            return await _context.Products.AnyAsync(p => p.Code == productCode);
        }

        public async Task<bool> ExistsByNameAsync(string productName)
        {
            return await _context.Products.AnyAsync(p => p.Name == productName);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.Include(m => m.Brand).Include(m => m.ProductImages).ToListAsync();
        }

        public async Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Products.Include(m=>m.Brand).Include(m=>m.ProductImages).AsQueryable();

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(Product product)
        {
            product.UpdatedOn = DateTime.Now;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
