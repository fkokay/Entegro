using Entegro.Data.Hooks;
using Entegro.Domain;
using Entegro.Domain.Entities.Catalog;
using Entegro.Engine;
using Entegro.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Service
{
    public class EngineWorker : BackgroundService
    {
        private readonly IEngineStarter _engineStarter;
        private readonly IServiceProvider _serviceProvider;

        public EngineWorker(IEngineStarter engineStarter, IServiceProvider serviceProvider)
        {
            _engineStarter = engineStarter;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Worker-friendly IApplicationBuilder
            var appBuilder = new WorkerApplicationBuilder(_serviceProvider);

            // EngineStarter’in init kodlarını çalıştır
            _engineStarter.ConfigureApplication(appBuilder);

            using var scope = _serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var lifetimeScopeAccessor = sp.GetRequiredService<ILifetimeScopeAccessor>();
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

            EngineContext.Current.Scope = new ScopedServiceContainer(
                lifetimeScopeAccessor,
                httpContextAccessor,
                sp.AsLifetimeScope()
            );

            // 2️⃣ DbContext’i al ve lazy loader etkinleştir
            var db = sp.GetRequiredService<EntegroDbContext>();

            // 3️⃣ Parent ve Children ilişkilerini yükle
            var parentCategory = await db.Categories
                .Include(c => c.Children)   // Child koleksiyonunu yükle
                .FirstOrDefaultAsync(c => c.Id == 1);

            if (parentCategory == null)
            {
                parentCategory = new Category
                {
                    Name = "Parent Kategori",
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };
                db.Categories.Add(parentCategory);
                await db.SaveChangesAsync();
            }

            // 4️⃣ Yeni category ekle (Added state)
            var newCategory = new Category
            {
                Name = "Yeni Alt Kategori",
                ParentId = parentCategory.Id,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            db.Categories.Add(newCategory);

            // 5️⃣ Explicit olarak parent-children ilişkisini set et
            parentCategory.Children.Add(newCategory);

            // 6️⃣ Kaydet ve hook tetiklet
            await db.SaveChangesAsync();

            Console.WriteLine("Yeni kategori eklendi ve TreeNodeHook tetiklendi.");

        }
    }
}