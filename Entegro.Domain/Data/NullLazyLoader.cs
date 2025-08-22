using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Data
{
    internal sealed class NullLazyLoader : ILazyLoader
    {
        public NullLazyLoader()
        {

        }

        public static NullLazyLoader Instance { get; } = new NullLazyLoader();

        public void SetLoaded(object entity, [CallerMemberName] string navigationName = "", bool loaded = true)
        {
        }

        public bool IsLoaded(object entity, [CallerMemberName] string navigationName = "")
            => true;

        public void Load(object entity, [CallerMemberName] string navigationName = "")
        {
        }

        public Task LoadAsync(object entity, CancellationToken cancellationToken = default, [CallerMemberName] string navigationName = "")
            => Task.CompletedTask;

        public void Dispose()
        {

        }
    }
}
