using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    [Table("ProductIntegration")]
    public class ProductIntegration : BaseEntity
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }

        private Product _product;
        public Product Product
        {
            get => _product ?? LazyLoader?.Load(this, ref _product);
            set => _product = value;
        }
        public int IntegrationSystemId { get; set; }

        private IntegrationSystem _integrationSystem;
        public IntegrationSystem IntegrationSystem
        {
            get => _integrationSystem ?? LazyLoader?.Load(this, ref _integrationSystem);
            set => _integrationSystem = value;
        }
        public string IntegrationCode { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; }

        //tekil product


    }
}
