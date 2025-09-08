using Entegro.Data.Hooks;
using Entegro.Domain.Entities.Catalog;
using Entegro.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Hooks
{
    public class CategorySaveHook : AsyncDbSaveHook<Category>
    {
        public override Task<HookResult> OnBeforeSaveAsync(IHookedEntity entry, CancellationToken cancelToken)
        {
            return base.OnBeforeSaveAsync(entry, cancelToken);
        }

        public override Task<HookResult> OnAfterSaveAsync(IHookedEntity entry, CancellationToken cancelToken)
        {
            return base.OnAfterSaveAsync(entry, cancelToken);
        }
    }
}
