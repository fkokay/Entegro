using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Entegro.Domain.Enums
{
    public enum OrderSource
    {
        [Description("Smartstore")]
        Smartstore = 1,
        [Description("Trendyol")]
        Trendyol = 2,
        [Description("Hepsiburada")]
        Hepsiburada = 3,
    }
    public enum OrderStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Processing
        /// </summary>
        Processing = 20,

        /// <summary>
        /// Complete
        /// </summary>
        Complete = 30,

        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 40
    }
    public enum ReturnRequestStatus
    {
        /// <summary>
        /// Beklemede
        /// </summary>
        [Display(Name = "Beklemede")]
        Pending = 0,

        /// <summary>
        /// Alındı
        /// </summary>
        [Display(Name = "Alındı")]
        Received = 10,

        /// <summary>
        /// İade Yetkilendirildi
        /// </summary>
        [Display(Name = "İade Yetkilendirildi")]
        ReturnAuthorized = 20,

        /// <summary>
        /// Ürün(ler) Onarıldı
        /// </summary>
        [Display(Name = "Ürün(ler) Onarıldı")]
        ItemsRepaired = 30,

        /// <summary>
        /// Ürün(ler) İade Edildi
        /// </summary>
        [Display(Name = "Ürün(ler) İade Edildi")]
        ItemsRefunded = 40,

        /// <summary>
        /// Talep Reddedildi
        /// </summary>
        [Display(Name = "Talep Reddedildi")]
        RequestRejected = 50,

        /// <summary>
        /// İptal Edildi
        /// </summary>
        [Display(Name = "İptal Edildi")]
        Cancelled = 60
    }
    public enum ReasonForReturnType
    {
        /// <summary>
        /// </summary>
        [Display(Name = "Yanlış Ürün Alındı")]
        ReceivedWrongProduct = 0,

        /// <summary>
        /// </summary>
        [Display(Name = "Yanlış Ürün Siparişi")]
        WrongProductOrdered = 1,

        /// <summary>
        /// </summary>
        [Display(Name = "Üründe Bir Sorun Var")]
        AProblemProduct = 2,


    }
    public enum RequestedActionType
    {
        /// <summary>
        /// </summary>
        [Display(Name = "Tamirat")]
        Repair = 0,

        /// <summary>
        /// </summary>
        [Display(Name = "Yenisiyle Değiştirme")]
        Replacement = 1,

        /// <summary>
        /// </summary>
        [Display(Name = "Mağaza Kredisi")]
        StoreCredit = 2,


    }

}
