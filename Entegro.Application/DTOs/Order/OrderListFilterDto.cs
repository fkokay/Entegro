namespace Entegro.Application.DTOs.Order
{
    public class OrderListFilterDto
    {
        public string CustomerName { get; set; }
        public string OrderNo { get; set; }
        public string PackageNo { get; set; }
        public string Barcode { get; set; }
        public string CargoCode { get; set; }
        public string ProductName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
