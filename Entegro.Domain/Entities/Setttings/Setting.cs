using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Setttings
{
    public class SettingMap : IEntityTypeConfiguration<Setting>
    {

        public void Configure(EntityTypeBuilder<Setting> builder)
        {

        }
    }

    [Table("Settings")]
    public class Setting : BaseEntity
    {
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
    }
}

