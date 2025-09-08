using AngleSharp.Dom;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Data.Hooks;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Catalog.Categories.Extensions;
using Entegro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Catalog.Categories
{
    [Important(HookImportance.Essential)]
    internal class TreeNodeHook : AsyncDbSaveHook<ITreeNode>
    {
        private readonly EntegroDbContext _db;
        private readonly ITreeNodeRepository<Category> _treeNodeRepository;
        public TreeNodeHook(EntegroDbContext db,ITreeNodeRepository<Category> treeNodeRepository)
        {
            _db = db;
            _treeNodeRepository = treeNodeRepository;
        }

        protected override async Task<HookResult> OnUpdatingAsync(ITreeNode entity, IHookedEntity entry, CancellationToken cancelToken)
        {
            if (entry.Entry.TryGetModifiedProperty(nameof(entity.ParentId),out var originalValue))
            {
                var category = entity as Category;

                var oldTreePath = entity.TreePath;
                var newTreePath = ITreeNodeExtensions.BuildTreePath<Category>(_treeNodeRepository, category, true);
                var query = _db.Categories.ApplyDescendantsFilter(entity);

                entity.TreePath = newTreePath;

                await query.ExecuteUpdateAsync(x => x.SetProperty(p => p.TreePath, p => p.TreePath.Replace(oldTreePath, newTreePath)), cancelToken);
            }
            return HookResult.Ok;
        }

        protected override Task<HookResult> OnInsertedAsync(ITreeNode entity, IHookedEntity entry, CancellationToken cancelToken)
        {
            return Task.FromResult(HookResult.Ok);
        }

        public override async Task OnAfterSaveCompletedAsync(IEnumerable<IHookedEntity> entries, CancellationToken cancelToken)
        {
            var nodes = entries.Where(x => x.Entity is ITreeNode).Select(x => x.Entity as ITreeNode).ToList();
            foreach (var node in nodes)
            {
                var category = node as Category;
                node.TreePath = ITreeNodeExtensions.BuildTreePath<Category>(_treeNodeRepository, category, true);
            }

            await _db.SaveChangesAsync(cancelToken);
        }
    }
}
