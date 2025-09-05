
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
        public virtual IntegrationSystem? IntegrationSystem { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
