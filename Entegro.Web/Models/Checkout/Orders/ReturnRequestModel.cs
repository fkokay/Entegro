using Entegro.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Web.Models.Checkout.Orders
{
    public class ReturnRequestModel
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public int CustomerId { get; set; }
        public CustomerModel? Customer { get; set; }
        public int Quantity { get; set; }
        public string ReasonForReturn { get; set; }
        public string RequestedAction { get; set; }
        public DateTime? RequestedActionUpdatedOn { get; set; }
        public string CustomerComments { get; set; }
        public string StaffNotes { get; set; }
        public string AdminComment { get; set; }
        public int ReturnRequestStatusId { get; set; }
        [NotMapped]
        public ReturnRequestStatus ReturnRequestStatus
        {
            get => (ReturnRequestStatus)ReturnRequestStatusId;
            set => ReturnRequestStatusId = (int)value;
        }
        public bool? RefundToWallet { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
