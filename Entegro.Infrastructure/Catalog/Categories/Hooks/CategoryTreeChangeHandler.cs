using Entegro.Application.Services.Base;
using Entegro.Caching;
using Entegro.Collections;
using Entegro.Data;
using Entegro.Data.Hooks;
using Entegro.Domain;
using Entegro.Domain.Entities.Catalog;
using Entegro.Events;
using Entegro.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Catalog.Categories.Hooks
{
    public enum CategoryTreeChangeReason
    {
        ElementCounts,
        Data,
        Hierarchy
    }

    public class CategoryTreeChangedEvent(CategoryTreeChangeReason reason)
    {
        public CategoryTreeChangeReason Reason { get; private set; } = reason;
    }

    internal class CategoryTreeChangeHandler : AsyncDbSaveHook<BaseEntity>, IConsumer
    {
        private static readonly string[] _h =
        [
             nameof(Category.ParentId),
             nameof(Category.Published),
             nameof(Category.Deleted),
             nameof(Category.DisplayOrder),
        ];

        private static readonly string[] _d =
        [
            nameof(Category.Name),
            nameof(Category.MediaFileId),
        ];

        private readonly EntegroDbContext _db;
        private readonly ICacheManager _cache;
        private readonly IEventPublisher _eventPublisher;


        private readonly bool[] _handledReasons = new bool[(int)CategoryTreeChangeReason.Hierarchy + 1];
        private bool _invalidated;
        public CategoryTreeChangeHandler(EntegroDbContext db, ICacheManager cache, IEventPublisher eventPublisher)
        {
            _db = db;
            _cache = cache;
            _eventPublisher = eventPublisher;
        }

        public override async Task<HookResult> OnBeforeSaveAsync(IHookedEntity entry, CancellationToken cancelToken)
        {
            if (entry.InitialState != Entegro.Data.EntityState.Modified)
            {
                return HookResult.Void;
            }

            var entity = entry.Entity;

            if (entity is Category category)
            {
                var modProps = _db.GetModifiedProperties(entity);

                if (modProps.Keys.Any(x => _h.Contains(x)))
                {
                    await _cache.RemoveByPatternAsync(CategoryService.CategoryTreePatternKey);
                    await PublishEvent(CategoryTreeChangeReason.Hierarchy);
                    _invalidated = true;
                }
                else if (modProps.Keys.Any(x => _d.Contains(x)))
                {
                    var publishEvent = false;
                    var keys = _cache.Keys(CategoryService.CategoryTreePatternKey).ToArray();

                    foreach (var key in keys)
                    {
                        var tree = await _cache.GetAsync<TreeNode<ICategoryNode>>(key);
                        if (tree != null)
                        {
                            var node = tree.SelectNodeById(entity.Id);
                            if (node != null)
                            {
                                publishEvent = true;
                                if (node.Value is CategoryNode value)
                                {
                                    value.Name = category.Name;
                                    value.MediaFileId = category.MediaFileId;

                                    await _cache.PutAsync(key, tree,new CacheEntryOptions().ExpiresIn(CategoryService.CategoryTreeCacheDuration));
                                }
                                else
                                {
                                    await _cache.RemoveAsync(key);
                                }
                            }
                        }
                    }


                    if (publishEvent)
                    {
                        await PublishEvent(CategoryTreeChangeReason.Data);
                    }
                }
            }
            else
            {
                return HookResult.Void;
            }

            return HookResult.Ok;
        }
        public override async Task<HookResult> OnAfterSaveAsync(IHookedEntity entry, CancellationToken cancelToken)
        {
            if (_invalidated)
            {
                return HookResult.Ok;
            }

            // INFO: Acl & StoreMapping affect element counts.

            var isNewOrDeleted = entry.InitialState == EntityState.Added || entry.InitialState == EntityState.Deleted;
            var entity = entry.Entity;

            if (entity is Category && isNewOrDeleted)
            {
                // INFO: 'Modified' case already handled in 'OnBeforeSave()'.
                // Hierarchy affecting change, nuke all.
                await _cache.RemoveByPatternAsync(CategoryService.CategoryTreePatternKey);
                await PublishEvent(CategoryTreeChangeReason.Hierarchy);
                _invalidated = true;
            }
            else
            {
                return HookResult.Void;
            }

            return HookResult.Ok;
        }

        private async Task PublishEvent(CategoryTreeChangeReason reason)
        {
            if (_handledReasons[(int)reason] == false)
            {
                await _eventPublisher.PublishAsync(new CategoryTreeChangedEvent(reason));
                _handledReasons[(int)reason] = true;
            }
        }
    }
}
