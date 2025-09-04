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

        public async Task<bool> ExistsByBarcodeAsync(string productBarcode)
        {
            return await _context.Products.AsNoTracking().AnyAsync(p => p.Code == productBarcode);
        }

        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            return await _context.Products.AsNoTracking().AnyAsync(p => p.Code == productCode);
        }

        public async Task<bool> ExistsByNameAsync(string productName)
        {
            return await _context.Products.AsNoTracking().AnyAsync(p => p.Name == productName);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var query = _context.Products
              .AsNoTracking()
              .AsQueryable();
            return await query.Select(m => new Product()
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
                ProductVariantAttributes = m.ProductVariantAttributes,
                ProductMediaFiles = m.ProductMediaFiles,
                ProductCategories = m.ProductCategories,
                ProductIntegrations = m.ProductIntegrations.Select(x => new ProductIntegration
                {
                    Active = x.Active,
                    Id = x.Id,
                    IntegrationSystem = new IntegrationSystem()
                    {
                        Id = x.IntegrationSystem.Id,
                        Description = x.IntegrationSystem.Description,
                        Name = x.IntegrationSystem.Name,
                        IntegrationSystemParameters = x.IntegrationSystem.IntegrationSystemParameters.Select(a => new IntegrationSystemParameter()
                        {
                            Id = a.Id,
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
                Width = m.Width,
                MainPictureId = m.MainPictureId,
                MainPicture = m.MainPicture == null ? null : new MediaFile()
                {
                    Alt = m.MainPicture.Alt,
                    CreatedOn = m.MainPicture.CreatedOn,
                    Deleted = m.MainPicture.Deleted,
                    Extension = m.MainPicture.Extension,
                    Folder = m.MainPicture.Folder,
                    FolderId = m.MainPicture.FolderId,
                    Height = m.MainPicture.Height,
                    Hidden = m.MainPicture.Hidden,
                    Id = m.MainPicture.Id,
                    IsTransient = m.MainPicture.IsTransient,
                    MediaType = m.MainPicture.MediaType,
                    Metadata = m.MainPicture.Metadata,
                    MimeType = m.MainPicture.MimeType,
                    Name = m.MainPicture.Name,
                    PixelSize = m.MainPicture.PixelSize,
                    Size = m.MainPicture.Size,
                    Title = m.MainPicture.Title,
                    UpdatedOn = m.MainPicture.UpdatedOn,
                    Version = m.MainPicture.Version,
                    Width = m.MainPicture.Width
                },

            }).ToListAsync();
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
                ProductVariantAttributes = m.ProductVariantAttributes,
                ProductMediaFiles = m.ProductMediaFiles,
                ProductCategories = m.ProductCategories,
                ProductIntegrations = m.ProductIntegrations.Select(x => new ProductIntegration
                {
                    Active = x.Active,
                    Id = x.Id,
                    IntegrationSystem = new IntegrationSystem()
                    {
                        Id = x.IntegrationSystem.Id,
                        Description = x.IntegrationSystem.Description,
                        Name = x.IntegrationSystem.Name,
                        IntegrationSystemParameters = x.IntegrationSystem.IntegrationSystemParameters.Select(a => new IntegrationSystemParameter()
                        {
                            Id = a.Id,
                            IntegrationSystemId = a.IntegrationSystemId,
                            Key = a.Key,
                            Value = a.Value,
                        }).ToList()
                    },
                    IntegrationSystemId = x.IntegrationSystemId,
                    LastSyncDate = x.LastSyncDate,
                    IntegrationCode = x.IntegrationCode,
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
                Width = m.Width,
                MainPictureId = m.MainPictureId,
                MainPicture = m.MainPicture == null ? null : new MediaFile()
                {
                    Alt = m.MainPicture.Alt,
                    CreatedOn = m.MainPicture.CreatedOn,
                    Deleted = m.MainPicture.Deleted,
                    Extension = m.MainPicture.Extension,
                    Folder = m.MainPicture.Folder,
                    FolderId = m.MainPicture.FolderId,
                    Height = m.MainPicture.Height,
                    Hidden = m.MainPicture.Hidden,
                    Id = m.MainPicture.Id,
                    IsTransient = m.MainPicture.IsTransient,
                    MediaType = m.MainPicture.MediaType,
                    Metadata = m.MainPicture.Metadata,
                    MimeType = m.MainPicture.MimeType,
                    Name = m.MainPicture.Name,
                    PixelSize = m.MainPicture.PixelSize,
                    Size = m.MainPicture.Size,
                    Title = m.MainPicture.Title,
                    UpdatedOn = m.MainPicture.UpdatedOn,
                    Version = m.MainPicture.Version,
                    Width = m.MainPicture.Width
                },

            })
                .OrderBy(m => m.Id)
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


        public async Task<Product?> GetByBarcodeAsync(string productBarcode)
        {

            return await _context.Products.AsNoTracking()
     .Include(m => m.ProductMediaFiles).ThenInclude(m => m.MediaFile).ThenInclude(m => m.Folder)
     .Include(m => m.ProductVariantAttributes).ThenInclude(m => m.ProductAttribute).ThenInclude(m => m.ProductAttributeValues)
     .Include(m => m.ProductVariantAttributeCombinations).FirstOrDefaultAsync(o => o.Barcode == productBarcode);
        }

        public async Task<Product?> GetByCodeAsync(string productCode)
        {
            return await _context.Products.AsNoTracking()
     .Include(m => m.ProductMediaFiles).ThenInclude(m => m.MediaFile).ThenInclude(m => m.Folder)
     .Include(m => m.ProductVariantAttributes).ThenInclude(m => m.ProductAttribute).ThenInclude(m => m.ProductAttributeValues)
     .Include(m => m.ProductVariantAttributeCombinations).FirstOrDefaultAsync(o => o.Code == productCode);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var product = await _context.Products.AsNoTracking().Select(m => new Product()
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
                ProductVariantAttributes = m.ProductVariantAttributes.Select(v => new ProductVariantAttribute
                {
                    AttributeControlTypeId = v.AttributeControlTypeId,
                    DisplayOrder = v.DisplayOrder,
                    Id = v.Id,
                    IsRequried = v.IsRequried,
                    ProductAttributeId = v.ProductAttributeId,
                    ProductId = v.ProductId,
                    ProductAttribute = new ProductAttribute
                    {
                        Id = v.ProductAttributeId,
                        Name = v.ProductAttribute.Name,
                        Description = v.ProductAttribute.Description,
                        DisplayOrder = v.ProductAttribute.DisplayOrder,
                        ProductAttributeValues = v.ProductAttribute.ProductAttributeValues.Select(v => new ProductAttributeValue
                        {
                            Id = v.ProductAttributeId,
                            Name = v.Name,
                            ProductAttributeId = v.ProductAttributeId,
                            DisplayOrder = v.DisplayOrder,
                        }).ToList()
                    },
                }).ToList(),
                ProductMediaFiles = m.ProductMediaFiles.Select(mf => new ProductMediaFile
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
                ProductCategories = m.ProductCategories.Select(pc => new ProductCategory
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
                ProductIntegrations = m.ProductIntegrations.Select(x => new ProductIntegration
                {
                    Active = x.Active,
                    Id = x.Id,
                    IntegrationSystem = new IntegrationSystem()
                    {
                        Id = x.IntegrationSystem.Id,
                        Description = x.IntegrationSystem.Description,
                        Name = x.IntegrationSystem.Name,
                        IntegrationSystemParameters = x.IntegrationSystem.IntegrationSystemParameters.Select(a => new IntegrationSystemParameter()
                        {
                            Id = a.Id,
                            IntegrationSystemId = a.IntegrationSystemId,
                            Key = a.Key,
                            Value = a.Value,
                        }).ToList()
                    },
                    IntegrationSystemId = x.IntegrationSystemId,
                    LastSyncDate = x.LastSyncDate,
                    IntegrationCode = x.IntegrationCode,
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
                Width = m.Width,
                MainPictureId = m.MainPictureId,
                MainPicture = m.MainPicture == null ? null : new MediaFile()
                {
                    Alt = m.MainPicture.Alt,
                    CreatedOn = m.MainPicture.CreatedOn,
                    Deleted = m.MainPicture.Deleted,
                    Extension = m.MainPicture.Extension,
                    Folder = m.MainPicture.Folder,
                    FolderId = m.MainPicture.FolderId,
                    Height = m.MainPicture.Height,
                    Hidden = m.MainPicture.Hidden,
                    Id = m.MainPicture.Id,
                    IsTransient = m.MainPicture.IsTransient,
                    MediaType = m.MainPicture.MediaType,
                    Metadata = m.MainPicture.Metadata,
                    MimeType = m.MainPicture.MimeType,
                    Name = m.MainPicture.Name,
                    PixelSize = m.MainPicture.PixelSize,
                    Size = m.MainPicture.Size,
                    Title = m.MainPicture.Title,
                    UpdatedOn = m.MainPicture.UpdatedOn,
                    Version = m.MainPicture.Version,
                    Width = m.MainPicture.Width
                },

            }).OrderBy(m => m.Id).FirstOrDefaultAsync(o => o.Id == id);


            return product;

            //return await _context.Products.AsNoTracking()
            //    .Include(m => m.ProductMediaFiles).ThenInclude(m => m.MediaFile).ThenInclude(m => m.Folder)
            //    .Include(m => m.ProductVariantAttributes).ThenInclude(m => m.ProductAttribute).ThenInclude(m => m.ProductAttributeValues)
            //    .Include(m => m.ProductVariantAttributeCombinations).FirstOrDefaultAsync(o => o.Id == id);
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
