using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class ProductIntegrationRepository : IProductIntegrationRepository
    {
        private readonly EntegroContext _context;

        public ProductIntegrationRepository(EntegroContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductIntegration productIntegration)
        {

            await _context.ProductIntegrations.AddAsync(productIntegration);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductIntegration productIntegration)
        {
            var model = await _context.ProductIntegrations.FindAsync(productIntegration.Id);
            if (model != null)
            {
                _context.ProductIntegrations.Remove(model);
                await _context.SaveChangesAsync();

            }

        }

        public async Task<List<ProductIntegration>> GetAllAsync()
        {
            return await _context.ProductIntegrations.AsNoTracking()
                .Include(m => m.IntegrationSystem).ThenInclude(m => m.IntegrationSystemParameters)
                .Include(m => m.Product).ThenInclude(m => m.Brand)
                .Include(m => m.Product.ProductCategories).ThenInclude(m => m.Category).ThenInclude(m => m.ParentCategory)
                .Include(m => m.Product.ProductMediaFiles).ThenInclude(m => m.MediaFile).ThenInclude(m => m.Folder)
                .Include(m => m.Product.ProductVariantAttributes).ThenInclude(m => m.ProductAttribute)
                .Include(m => m.Product.ProductVariantAttributes).ThenInclude(m => m.ProductVariantAttributeValues)
                .Include(m => m.Product.ProductVariantAttributeCombinations).ToListAsync();
        }

        public async Task<PagedResult<ProductIntegration>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ProductIntegrations.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var orders = await query
                .OrderBy(b => b.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductIntegration>
            {
                Items = orders,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductIntegration?> GetByIdAsync(int id)
        {
            return await _context.ProductIntegrations.Select(pi => new ProductIntegration
            {
                Id = pi.Id,
                Active = pi.Active,
                ProductId = pi.ProductId,
                LastSyncDate = pi.LastSyncDate,
                Custom = pi.Custom,
                IntegrationCode = pi.IntegrationCode,
                IntegrationSystemId = pi.IntegrationSystemId,
                IsSync = pi.IsSync,
                Price = pi.Price,
                IntegrationSystem = new IntegrationSystem
                {
                    Id = pi.IntegrationSystemId,
                    Description = pi.IntegrationSystem.Description,
                    Name = pi.IntegrationSystem.Name,
                    IntegrationSystemTypeId = pi.IntegrationSystem.IntegrationSystemTypeId,
                    IntegrationSystemType = pi.IntegrationSystem.IntegrationSystemType,
                    IntegrationSystemParameters = pi.IntegrationSystem.IntegrationSystemParameters.Select(i => new IntegrationSystemParameter
                    {
                        Id = i.Id,
                        IntegrationSystemId = i.IntegrationSystemId,
                        Key = i.Key,
                        Value = i.Value,
                    }).ToList(),
                    IntegrationSystemLogs = pi.IntegrationSystem.IntegrationSystemLogs.Select(i => new IntegrationSystemLog
                    {
                        Id = i.Id,
                        Exception = i.Exception,
                        Timestamp = i.Timestamp,
                        IntegrationSystemId = i.IntegrationSystemId,
                        LogLevel = i.LogLevel,
                        Message = i.Message,
                    }).ToList(),
                },
                Product = new Product
                {
                    Id = pi.Product.Id,
                    Barcode = pi.Product.Barcode,
                    BrandId = pi.Product.BrandId,
                    Code = pi.Product.Code,
                    Currency = pi.Product.Currency,
                    Gtin = pi.Product.Gtin,
                    MainPictureId = pi.Product.MainPictureId,
                    Unit = pi.Product.Unit,
                    CreatedOn = pi.Product.CreatedOn,
                    Height = pi.Product.Height,
                    Deleted = pi.Product.Deleted,
                    Price = pi.Product.Price,
                    Description = pi.Product.Description,
                    Length = pi.Product.Length,
                    Width = pi.Product.Width,
                    VatInc = pi.Product.VatInc,
                    Weight = pi.Product.Weight,
                    VatRate = pi.Product.VatRate,
                    UpdatedOn = pi.Product.UpdatedOn,
                    StockQuantity = pi.Product.StockQuantity,
                    Name = pi.Product.Name,
                    SpecialPrice = pi.Product.SpecialPrice,
                    Published = pi.Product.Published,
                    OldPrice = pi.Product.OldPrice,
                    MetaKeywords = pi.Product.MetaKeywords,
                    MetaTitle = pi.Product.MetaTitle,
                    MetaDescription = pi.Product.MetaDescription,
                    ManufacturerPartNumber = pi.Product.ManufacturerPartNumber,
                    Brand = new Brand
                    {
                        Id = pi.Product.Brand.Id,
                        Name = pi.Product.Brand.Name
                    },
                    ProductCategories = pi.Product.ProductCategories.Select(pc => new ProductCategory
                    {
                        Id = pc.Id,
                        CategoryId = pc.CategoryId,
                        Category = new Category
                        {
                            Id = pc.CategoryId,
                            Name = pc.Category.Name,
                            ParentCategory = pc.Category.ParentCategory == null ? null : new Category()
                            {
                                Id = pc.Category.ParentCategoryId.Value,
                                Name = pc.Category.Name,
                            }
                        },
                        ProductId = pc.ProductId,
                        DisplayOrder = pc.DisplayOrder,
                    }).ToList(),
                    ProductMediaFiles = pi.Product.ProductMediaFiles.Select(mf => new ProductMediaFile
                    {
                        Id = mf.Id,
                        DisplayOrder = mf.DisplayOrder,
                        MediaFileId = mf.MediaFileId,
                        ProductId = mf.ProductId,
                        MediaFile = new MediaFile
                        {
                            Id = mf.MediaFile.Id,
                            Alt = mf.MediaFile.Alt,
                            CreatedOn = mf.MediaFile.CreatedOn,
                            Deleted = mf.MediaFile.Deleted,
                            Extension = mf.MediaFile.Extension,
                            FolderId = mf.MediaFile.FolderId,
                            Height = mf.MediaFile.Height,
                            Width = mf.MediaFile.Width,
                            Version = mf.MediaFile.Version,
                            Hidden = mf.MediaFile.Hidden,
                            IsTransient = mf.MediaFile.IsTransient,
                            MimeType = mf.MediaFile.MimeType,
                            Name = mf.MediaFile.Name,
                            Title = mf.MediaFile.Title,
                            PixelSize = mf.MediaFile.PixelSize,
                            Size = mf.MediaFile.Size,
                            Metadata = mf.MediaFile.Metadata,
                            UpdatedOn = mf.MediaFile.UpdatedOn,
                            MediaType = mf.MediaFile.MimeType,
                            Folder = mf.MediaFile == null ? null : new MediaFolder
                            {
                                Id = mf.MediaFile.FolderId.Value,
                                Metadata = mf.MediaFile.Folder.Metadata,
                                Discriminator = mf.MediaFile.Folder.Discriminator,
                                CanDetectTracks = mf.MediaFile.Folder.CanDetectTracks,
                                TreePath = mf.MediaFile.Folder.TreePath,
                                FilesCount = mf.MediaFile.Folder.FilesCount,
                                IncludePath = mf.MediaFile.Folder.IncludePath,
                                MediaFiles = mf.MediaFile.Folder.MediaFiles,
                                Order = mf.MediaFile.Folder.Order,
                                Name = mf.MediaFile.Folder.Name,
                                Slug = mf.MediaFile.Folder.Slug,
                                ParentId = mf.MediaFile.Folder.ParentId,
                                Parent = mf.MediaFile.Folder.Parent,
                                ResKey = mf.MediaFile.Folder.ResKey
                            }
                        }
                    }).ToList(),
                    ProductVariantAttributes = pi.Product.ProductVariantAttributes.Select(pv => new ProductVariantAttribute
                    {
                        Id = pv.Id,
                        AttributeControlTypeId = pv.AttributeControlTypeId,
                        DisplayOrder = pv.AttributeControlTypeId,
                        IsRequried = pv.IsRequried,
                        ProductId = pv.ProductId,
                        ProductAttributeId = pv.AttributeControlTypeId,
                        ProductAttribute = new ProductAttribute
                        {
                            Id = pv.ProductAttribute.Id,
                            DisplayOrder = pv.ProductAttribute.DisplayOrder,
                            Description = pv.ProductAttribute.Description,
                            Name = pv.ProductAttribute.Name,
                            ProductAttributeValues = pv.ProductAttribute.ProductAttributeValues.Select(pav => new ProductAttributeValue
                            {
                                Id = pav.Id,
                                DisplayOrder = pav.DisplayOrder,
                                Name = pav.Name,
                                ProductAttributeId = pav.ProductAttributeId
                            }).ToList(),
                        }
                    }).ToList(),
                    ProductVariantAttributeCombinations = pi.Product.ProductVariantAttributeCombinations.Select(c => new ProductVariantAttributeCombination
                    {
                        Id = c.Id,
                        StokCode = c.StokCode,
                        Gtin = c.Gtin,
                        StockQuantity = c.StockQuantity,
                        Price = c.Price,
                        ProductId = c.ProductId,
                        RawAttribute = c.RawAttribute,
                        ManufacturerPartNumber = c.ManufacturerPartNumber,
                    }).ToList(),

                }
            }).AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

        }

        public async Task<ProductIntegration?> GetByIntegrationCodeAsync(string integrationCode)
        {
            return await _context.ProductIntegrations
                .Include(c => c.Product).AsNoTracking()
                .FirstOrDefaultAsync(t => t.IntegrationCode == integrationCode);
        }

        public async Task<ProductIntegration?> GetByIntegrationSystemIdandIntegrationCodeAsync(int integrationSystemId, string integrationCode)
        {
            var productIntegration = await _context.ProductIntegrations
               .Include(p => p.Product)
               .AsNoTracking()
               .FirstOrDefaultAsync(p =>
                   p.IntegrationSystemId == integrationSystemId &&
                   p.IntegrationCode == integrationCode);
            return productIntegration;
        }

        public async Task<ProductIntegration?> GetByProductIdandIntegrationSystemIdAsync(int productId, int integrationSystemId)
        {

            return await _context.ProductIntegrations.Include(c => c.Product).AsNoTracking()
                .FirstOrDefaultAsync(t => t.ProductId == productId && t.IntegrationSystemId == integrationSystemId);
        }

        public async Task UpdateAsync(ProductIntegration productIntegration)
        {
            _context.ProductIntegrations.Update(productIntegration);
            await _context.SaveChangesAsync();

        }
    }
}
