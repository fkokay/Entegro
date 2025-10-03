namespace Entegro.Web.Models.Setting
{
    public class CreateGeneralCommonModel
    {
        public string SystemUrl { get; set; }
        public string SystemApiUrl { get; set; }

        public string DecreaseStockOnOrderParameter { get; set; } //Sipariş geldiğinde stok azalsın mı?
        public string IncreaseStockOnCancelParameter { get; set; }//Sipariş iptal olduğunda stok geri artsın mı?
    }
}
