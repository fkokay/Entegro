using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Domain.Entities;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EntegroContext _context;

        public CategoryRepository(EntegroContext context)
        {
            _context = context;
        }


        //Yeni Kategori Eklerken TreePath değeri için category.Id kullanılıyor ancak category.Id henüz veritabanına eklenmediği için 0 olabilir.
        public async Task AddAsync(Category category)
        {
            try
            {
                category.CreatedOn = DateTime.UtcNow;
                category.UpdatedOn = DateTime.UtcNow;
                category.TreePath = $"/{category.Id}/";

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        // Silme işlemi
        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }


        // İsim ile varlık kontrolü
        public async Task<bool> ExistsByNameAsync(string name) => await _context.Categories.AnyAsync(o => o.Name == name);
        // Tüm kategorileri getir
        public async Task<List<Category>> GetAllAsync() => await _context.Categories.AsNoTracking().ToListAsync();


        // Sayfalı kategori listeleme
        public async Task<PagedResult<Category>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Categories.Include(m => m.MediaFile).ThenInclude(m => m.Folder).AsNoTracking().AsQueryable();

            var totalCount = await query.CountAsync();
            var categories = await query.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<Category>
            {
                Items = categories,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        // ID ile kategori getir
        public async Task<Category?> GetByIdAsync(int id) => await _context.Categories.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);


        // ID ile kategori ve ilişkili medya dosyasını getir
        public async Task<Category?> GetByIdWithMediaAsync(int id)
        {
            return await _context.Categories
             .Include(b => b.MediaFile)
             .ThenInclude(b => b.Folder)
             .FirstOrDefaultAsync(b => b.Id == id);
        }


        // İsim ile kategori getir
        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
            .Include(b => b.MediaFile)
            .ThenInclude(b => b.Folder)
            .FirstOrDefaultAsync(b => b.Name == name);
        }

        // Belirli bir üst kategori ID'sine sahip kategorileri getir
        public async Task<List<Category>> GetByParentIdAsync(int parentCategoryId)
        {
            return await _context.Categories.Where(c => c.ParentCategoryId == parentCategoryId).ToListAsync();
        }

        // ID'lere göre kategori isimlerini getir
        public async Task<Dictionary<int, string>> GetNamesByIdsAsync(IEnumerable<int> ids)
        {

            var set = ids.Distinct().ToArray();
            return await _context.Categories
                .AsNoTracking()
                .Where(c => set.Contains(c.Id))
                .Select(c => new { c.Id, c.Name })
                .ToDictionaryAsync(k => k.Id, v => v.Name);
        }

        // Kategori arama ve sayfalama
        public async Task<PagedResult2<CategorySlim>> SearchPagedAsync(string? term, int page, int pageSize)
        {


            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var q = _context.Categories.AsNoTracking();


            // Arama terimi boş değilse filtre uygula
            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                // Türkçe karakter duyarsız arama için collation kullan
                q = q.Where(c => EF.Functions.Collate(c.Name, "Turkish_CI_AI").Contains(term));
                if (int.TryParse(term, out var idVal))
                {
                    q = q.Union(_context.Categories.AsNoTracking().Where(c => c.Id == idVal));
                }
            }
            // Sıralama: Önce DisplayOrder, sonra Name, sonra Id
            q = q.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ThenBy(c => c.Id);

            var skip = (page - 1) * pageSize;

            var rows = await q.Select(c => new CategorySlim { Id = c.Id, Name = c.Name, TreePath = c.TreePath })
                              .Skip(skip)
                              .Take(pageSize + 1) // +1 => hasMore
                              .ToListAsync();

            var hasMore = rows.Count > pageSize;
            if (hasMore) rows.RemoveAt(pageSize);

            return new PagedResult2<CategorySlim>
            {
                Items = rows,
                HasMore = hasMore
            };
        }
        // Kategori güncelleme
        public async Task UpdateAsync(Category category)
        {
            category.UpdatedOn = DateTime.UtcNow;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}


