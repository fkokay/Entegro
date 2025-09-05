using Microsoft.EntityFrameworkCore;

namespace Entegro.Data
{
    public interface IDbContextConfigurationSource<TContext>
        where TContext : DbContext
    {
        void Configure(IServiceProvider services, DbContextOptionsBuilder builder);
    }
}
