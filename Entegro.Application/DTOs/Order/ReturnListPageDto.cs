namespace Entegro.Application.DTOs.Order
{
    public class ReturnListPageDto
    {
        public int CreatedQuantity { get; set; }
        public int WaitingInActionQuantity { get; set; }
        public int WaitingFraudCheckQuantity { get; set; }
        public int UnresolvedQuantity { get; set; }
        public int RejectedQuantity { get; set; }
        public int AcceptedQuantity { get; set; }
        public int CancelledQuantity { get; set; }
        public int InAnalysisQuantity { get; set; }
    }
}
