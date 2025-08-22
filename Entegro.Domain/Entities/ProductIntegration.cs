using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    [Table("ProductIntegration")]
    public class ProductIntegration : BaseEntity
    {
        public int ProductId { get; set; }
        public int IntegrationSystemId { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; }

        //tekil product

        private Product? _product;
        public Product? Product
        {
            get => _product ?? LazyLoader?.Load(this, ref _product);
            set => _product = value;
        }
    }
}
