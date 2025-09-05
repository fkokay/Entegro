using Microsoft.EntityFrameworkCore;

namespace Entegro.Data.Migrations
{
    public sealed class SeedingDbMigrationEvent
    {
        public long MigrationVersion { get; internal set; }
        public string MigrationDescription { get; internal set; }

        public DbContext DbContext { get; internal set; }
    }

    public sealed class SeededDbMigrationEvent
    {
        public long MigrationVersion { get; internal set; }
        public string MigrationDescription { get; internal set; }

        public DbContext DbContext { get; internal set; }
    }
}
