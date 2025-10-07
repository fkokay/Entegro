using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace Entegro.Infrastructure.Repositories
{
    public class CrossSellProductRepository : ICrossSellProductRepository
    {
        private readonly EntegroDbContext _context;

        public CrossSellProductRepository(EntegroDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CrossSellProduct crossSellProduct)
        {
            await _context.CrossSellProducts.AddAsync(crossSellProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllAsync(List<CrossSellProduct> crossSellProduct)
        {
            _context.CrossSellProducts.RemoveRange(crossSellProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CrossSellProduct crossSellProduct)
        {
            var entity = new CrossSellProduct { Id = crossSellProduct.Id };
            _context.CrossSellProducts.Attach(entity);
            _context.CrossSellProducts.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(int productId1, int productId2)
        {
            return await _context.CrossSellProducts.AnyAsync(o => o.ProductId1 == productId1 && o.ProductId2 == productId2);
        }

        public async Task<CrossSellProduct?> GetByIdAsync(int id)
        {
            return await _context.CrossSellProducts.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<CrossSellProduct?> GetByIdAsync(int productId1, int productId2)
        {
            return await _context.CrossSellProducts
             .Include(b => b.Product1)
             .Include(b => b.Product2)
             .AsNoTracking().FirstOrDefaultAsync(o => o.ProductId1 == productId1 && o.ProductId2 == productId2);
        }

        public async Task<Application.DTOs.Common.PagedResult<CrossSellProduct>> GetPagedAsync(GridCommand gridCommand, int productId)
        {
            var query = _context.CrossSellProducts.Include(b => b.Product1).Include(b => b.Product2).OrderBy(b => b.Id).Where(b => b.ProductId1 == productId).AsNoTracking();


            if (gridCommand.Columns != null)
            {
                foreach (var col in gridCommand.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Search?.Value))
                    {
                        var searchVal = col.Search.Value.Trim('^', '$');

                        // ilgili property’nin tipini bul
                        var prop = typeof(Brand).GetProperty(col.Data);
                        if (prop == null) continue;

                        if (prop.PropertyType == typeof(string))
                        {
                            query = query.Where($"{col.Data}.Contains(@0)", searchVal);
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                        {
                            if (int.TryParse(searchVal, out var intVal))
                                query = query.Where($"{col.Data} == @0", intVal);
                        }
                        else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                        {
                            if (bool.TryParse(searchVal, out var boolVal))
                                query = query.Where($"{col.Data} == @0", boolVal);
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (DateTime.TryParse(searchVal, out var dt))
                                query = query.Where($"{col.Data}.Date == @0", dt.Date);
                        }
                    }
                }
            }

            if (gridCommand.Search != null)
            {
                if (!string.IsNullOrEmpty(gridCommand.Search.Value))
                {
                    query = query.Where(b =>
                    b.Product1.Name.Contains(gridCommand.Search.Value) || b.Product2.Name.Contains(gridCommand.Search.Value)).AsQueryable();
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

            var crossSellProducts = await query
                .Skip(gridCommand.Start)
                .Take(gridCommand.Length)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            return new Application.DTOs.Common.PagedResult<CrossSellProduct>
            {
                Items = crossSellProducts,
                TotalCount = totalCount,
                PageNumber = gridCommand.Start / gridCommand.Length,
                PageSize = gridCommand.Length
            };
        }

        public async Task UpdateAsync(CrossSellProduct crossSellProduct)
        {
            _context.CrossSellProducts.Update(crossSellProduct);
            await _context.SaveChangesAsync();
        }
    }
}
