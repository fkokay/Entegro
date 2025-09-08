using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductVariantAttributeValueRepository : IProductVariantAttributeValueRepository
    {
        private readonly EntegroDbContext _context;

        public ProductVariantAttributeValueRepository(EntegroDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(ProductVariantAttributeValue data)
        {
            await _context.ProductVariantAttributeValues.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductVariantAttributeValue?> GetByNameAsync(string name)
        {
            return await _context.ProductVariantAttributeValues.FirstOrDefaultAsync(o => o.Name == name);
        }
    }
}
