using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities.Content;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly EntegroDbContext _context;

        public MediaFileRepository(EntegroDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(MediaFile mediaFile)
        {
            await _context.MediaFiles.AddAsync(mediaFile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MediaFile mediaFile)
        {
            _context.MediaFiles.Remove(mediaFile);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MediaFile>> GetAllAsync()
        {
            return await _context.MediaFiles.AsNoTracking().ToListAsync();
        }

        public async Task<PagedResult<MediaFile>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.MediaFiles.AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderBy(b => b.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<MediaFile>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<MediaFile?> GetByIdAsync(int id)
        {
            return await _context.MediaFiles.Include(m => m.MediaFolder).AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<MediaFile?> GetByNameAndFolderAsync(string name, int? folderId)
        {
            return await _context.MediaFiles.Include(m => m.MediaFolder).AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name == name && x.MediaFolderId == folderId);
        }

        public async Task UpdateAsync(MediaFile mediaFile)
        {
            _context.MediaFiles.Update(mediaFile);
            await _context.SaveChangesAsync();
        }
    }
}
