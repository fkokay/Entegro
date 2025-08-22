using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
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
