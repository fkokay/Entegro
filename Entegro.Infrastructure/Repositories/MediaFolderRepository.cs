using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class MediaFolderRepository : IMediaFolderRepository
    {
        private readonly EntegroContext _context;

        public MediaFolderRepository(EntegroContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(MediaFolder mediaFolder)
        {
            await _context.MediaFolders.AddAsync(mediaFolder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MediaFolder mediaFolder)
        {
            _context.MediaFolders.Remove(mediaFolder);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MediaFolder>> GetAllAsync()
        {
            return await _context.MediaFolders.ToListAsync();
        }

        public async Task<PagedResult<MediaFolder>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.MediaFolders.AsQueryable();

            var totalCount = await query.CountAsync();
            var customers = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<MediaFolder>
            {
                Items = customers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<MediaFolder?> GetByIdAsync(int id)
        {
            return await _context.MediaFolders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(MediaFolder mediaFolder)
        {
            _context.MediaFolders.Update(mediaFolder);
            await _context.SaveChangesAsync();
        }
    }
}
