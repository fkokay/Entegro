using System.ComponentModel.DataAnnotations;

namespace Entegro.Domain.Enums
{
    public enum IntegrationSystemType
    {
        [Display(Name = "Yok")]
        None = 0,
        [Display(Name = "ERP Entegrasyonu")]
        ERP = 1,
        [Display(Name = "E-Ticaret Entegrasyonu")]
        Commerce = 2,
        [Display(Name = "Pazaryeri Entegrasyonu")]
        Marketplace = 3,
        [Display(Name = "Kargo Entegrasyonu")]
        Cargo = 4,
        [Display(Name = "E-Fatura Entegrasyonu")]
        EInvoice = 5,
        [Display(Name = "Mail Entegrasyonu")]
        EMail = 6,
    }
}
