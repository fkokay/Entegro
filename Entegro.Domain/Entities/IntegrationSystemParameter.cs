using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    public class IntegrationSystemParameterMap : IEntityTypeConfiguration<IntegrationSystemParameter>
    {
        public void Configure(EntityTypeBuilder<IntegrationSystemParameter> builder)
        {

        }
    }
    [Table("IntegrationSystemParameter")]
    public class IntegrationSystemParameter : BaseEntity
    {
        public int IntegrationSystemId { get; set; }

        private IntegrationSystem? _integrationSystem;
        public IntegrationSystem? IntegrationSystem
        {
            get => _integrationSystem ?? LazyLoader?.Load(this, ref _integrationSystem);
            set => _integrationSystem = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
