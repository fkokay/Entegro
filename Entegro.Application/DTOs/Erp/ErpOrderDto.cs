using Entegro.Application.DTOs.Address;

namespace Entegro.Application.DTOs.Erp
{
    public class ErpOrderDto
    {
        public string OrderNumber { get; set; }
        public string OrderGuid { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal OrderSubTotal { get; set; }
        public decimal OrderShipping { get; set; }
        public decimal PaymentFee { get; set; }
        public decimal OrderTax { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal RefundedAmount { get; set; }
        public string IntegrationOrderNumber { get; set; }
        public string ShippingMethod { get; set; }
        public int? ShippingAddressId { get; set; }

        public AddressDto InvoiceAddress { get; set; }
        public AddressDto ShippingAddress { get; set; }

        public List<ErpOrderItemDto> OrderItems { get; set; } = new List<ErpOrderItemDto>();
    }
}

