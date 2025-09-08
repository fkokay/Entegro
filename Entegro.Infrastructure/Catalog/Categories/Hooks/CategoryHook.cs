using Entegro.Application.Services;
using Entegro.Caching;
using Entegro.Data;
using Entegro.Data.Hooks;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityState = Entegro.Data.EntityState;

namespace Entegro.Infrastructure.Catalog.Categories
{
    [Important]
    internal class CategoryHook : AsyncDbSaveHook<Category>
    {
        private readonly EntegroDbContext _db;
        private readonly IRequestCache _requestCache;
        public CategoryHook(EntegroDbContext db, IRequestCache requestCache)
        {
            _db = db;
            _requestCache = requestCache;
        }

        protected override Task<HookResult> OnInsertedAsync(Category entity, IHookedEntity entry, CancellationToken cancelToken) => Task.FromResult(HookResult.Ok);
        protected override Task<HookResult> OnUpdatedAsync(Category entity, IHookedEntity entry, CancellationToken cancelToken) => Task.FromResult(HookResult.Ok);
        public override async Task OnBeforeSaveCompletedAsync(IEnumerable<IHookedEntity> entries, CancellationToken cancelToken)
        {
            var invalidCategoryIds = new HashSet<int>();
            var modifiedCategories = entries
                .Where(x => x.InitialState == EntityState.Modified)
                .Select(x => x.Entity)
                .OfType<Category>()
                .ToList();

            foreach (var category in modifiedCategories)
            {
                if (!await IsValidCategoryHierarchy(category.Id, category.ParentId, cancelToken))
                {
                    invalidCategoryIds.Add(category.Id);
                }
            }

            if (invalidCategoryIds.Count > 0)
            {
                await _db.Categories
                    .Where(x => invalidCategoryIds.Contains(x.Id))
                    .ExecuteUpdateAsync(x => x.SetProperty(p => p.ParentId, p => null), cancelToken);
            }

            _requestCache.RemoveByPattern(CategoryService.CategoriesPatternKey);
        }

        private async Task<bool> IsValidCategoryHierarchy(int categoryId, int? parentCategoryId, CancellationToken cancelToken)
        {
            var parent = await _db.Categories
                .Where(x => x.Id == parentCategoryId)
                .Select(x => new { x.Id, x.ParentId })
                .FirstOrDefaultAsync(cancelToken);

            while (parent?.Id > 0)
            {
                if (categoryId == parent.Id)
                {
                    // Same ID = invalid.
                    return false;
                }

                if (parent.ParentId == null)
                {
                    break;
                }

                parent = await _db.Categories
                    .Where(x => x.Id == parent.ParentId)
                    .Select(x => new { x.Id, x.ParentId })
                    .FirstOrDefaultAsync(cancelToken);
            }

            return true;
        }
    }
}
