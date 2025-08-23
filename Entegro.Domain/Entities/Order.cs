using Entegro.Domain.Common;
using Entegro.Domain.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    [Table("Order")]
    public class Order : BaseEntity, ISoftDeletable, ITransient
    {
        public int OrderSourceId { get; set; }
        [NotMapped]
        public OrderSource OrderSource
        {
            get => (OrderSource)OrderSourceId;
            set => OrderSourceId = (int)value;
        }
        [NotMapped]
        public string OrderSourceLabelHint
        {
            get
            {
                return OrderSource switch
                {
                    OrderSource.Smartstore => "Smartstore"
                };
            }
        }
        public string OrderNo { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Deleted { get; set; }
        public bool IsTransient { get; set; }
        private Customer? _customer;
        public Customer? Customer
        {
            get => _customer ?? LazyLoader?.Load(this, ref _customer);
            set => _customer = value;
        }
        private ICollection<OrderItem> _orderItems;
        public ICollection<OrderItem> OrderItems
        {
            get => LazyLoader?.Load(this, ref _orderItems) ?? (_orderItems ??= new HashSet<OrderItem>());
            set => _orderItems = value;
        }
    }
}
