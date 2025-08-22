using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Products
                .Include(m => m.Brand)
                .Include(m => m.ProductMediaFiles)
                .Include(m => m.ProductIntegrations)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PagedResult<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Products
                .AsNoTracking()
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var products = await query.Select(m => new Product()
            {
                Id = m.Id,
                Barcode = m.Barcode,
                Name = m.Name,
                Gtin = m.Gtin,
                Price = m.Price,
                Brand = m.Brand,
                BrandId = m.BrandId,
                Code = m.Code,
                CreatedOn = m.CreatedOn,
                Currency = m.Currency,
                Deleted = m.Deleted,
                Description = m.Description,
                Height = m.Height,
                Length = m.Length,
                ManufacturerPartNumber = m.ManufacturerPartNumber,
                MetaDescription = m.MetaDescription,
                MetaKeywords = m.MetaKeywords,
                MetaTitle = m.MetaTitle,
                OldPrice = m.OldPrice,
                ProductVariantAttribute = m.ProductVariantAttribute,
                ProductMediaFiles = m.ProductMediaFiles,
                ProductCategories = m.ProductCategories,
                ProductIntegrations = m.ProductIntegrations.Select(x=> new ProductIntegration
                {
                    Active = x.Active,
                    Id = x.Id,
                    IntegrationSystem = new IntegrationSystem()
                    {
                        Id =x.IntegrationSystem.Id,
                        Description = x.IntegrationSystem.Description,
                        Name = x.IntegrationSystem.Name,
                        IntegrationSystemParameters = x.IntegrationSystem.IntegrationSystemParameters.Select(a=> new IntegrationSystemParameter()
                        {
                            Id=a.Id,
                            IntegrationSystemId = a.IntegrationSystemId,
                            Key = a.Key,
                            Value = a.Value,
                        }).ToList()
                    },
                    IntegrationSystemId = x.IntegrationSystemId,
                    LastSyncDate = x.LastSyncDate,
                    ProductId = x.ProductId,
                    Price = x.Price,
                }).ToList(),
                Published = m.Published,
                SpecialPrice = m.SpecialPrice,
                StockQuantity = m.StockQuantity,
                Unit = m.Unit,
                UpdatedOn = m.UpdatedOn,
                VatInc = m.VatInc,
                VatRate = m.VatRate,
                Weight = m.Weight,
                Width = m.Width
            })
                .Skip(pageNumber * pageSize)
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
            return await _context.Products.AsNoTracking()
                .Include(m => m.ProductMediaFiles).ThenInclude(m => m.MediaFile).ThenInclude(m => m.Folder)
                .Include(m => m.ProductVariantAttribute).ThenInclude(m => m.ProductAttribute).ThenInclude(m => m.ProductAttributeValues)
                .Include(m => m.ProductVariantAttributeCombination).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(Product product)
        {
            product.UpdatedOn = DateTime.Now;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMainPictureIdAsync(int productId, int mainPictureId)
        {
            var product = await _context.Products.Where(m => m.Id == productId).FirstAsync();
            product.MainPictureId = mainPictureId;
            product.UpdatedOn = DateTime.Now;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
