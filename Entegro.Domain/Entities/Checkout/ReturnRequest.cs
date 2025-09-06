using Entegro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Checkout
{
    [Table("ReturnRequest")]
    public class ReturnRequest :BaseEntity, IAuditable
    {
        public int OrderItemId { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }  
        public int Quantity { get; set; }
        [Required, StringLength(4000)]
        public string ReasonForReturn { get; set; }
        [Required, StringLength(4000)]
        public string RequestedAction { get; set; }
        public DateTime? RequestedActionUpdatedOnUtc { get; set; }
        public string CustomerComments { get; set; }
        public string StaffNotes { get; set; }
        [StringLength(4000)]
        public string AdminComment { get; set; }
        public int ReturnRequestStatusId { get; set; }
        [NotMapped]
        public ReturnRequestStatus ReturnRequestStatus
        {
            get => (ReturnRequestStatus)ReturnRequestStatusId;
            set => ReturnRequestStatusId = (int)value;
        }

        public bool? RefundToWallet { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedOnUtc { get; set; }

        /// <inheritdoc/>
        public DateTime UpdatedOnUtc { get; set; }
    }
}
