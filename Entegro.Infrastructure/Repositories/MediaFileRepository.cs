using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly EntegroContext _context;

        public MediaFileRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(MediaFile mediaFolder)
        {
            await _context.MediaFiles.AddAsync(mediaFolder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MediaFile mediaFolder)
        {
            _context.MediaFiles.Remove(mediaFolder);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MediaFile>> GetAllAsync()
        {
            return await _context.MediaFiles.ToListAsync();
        }

        public async Task<PagedResult<MediaFile>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.MediaFiles.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
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
            return await _context.MediaFiles.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<MediaFile?> GetByNameAndFolderAsync(string name, int folderId) => await _context.MediaFiles
                    .FirstOrDefaultAsync(x => x.Name == name && x.FolderId == folderId);

        public async Task UpdateAsync(MediaFile mediaFolder)
        {
            _context.MediaFiles.Update(mediaFolder);
            await _context.SaveChangesAsync();
        }
    }
}
