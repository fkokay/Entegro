namespace Entegro.Application.DTOs.Order
{
    public class ReturnRequestListFilterDto
    {
        public string CustomerName { get; set; }
        public string OrderNo { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnReason { get; set; }
        public string Barcode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
