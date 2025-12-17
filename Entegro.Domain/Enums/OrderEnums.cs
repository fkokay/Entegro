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
        /// Talep oluşturuldu, işlem bekliyor
        /// </summary>
        [Display(Name = "Beklemede")]
        Created = 0,

        /// <summary>
        /// Talep kabul edildi
        /// </summary>
        [Display(Name = "Kabul Edildi")]
        Accepted = 10,

        /// <summary>
        /// Talep reddedildi
        /// </summary>
        [Display(Name = "Reddedildi")]
        Rejected = 20,

        /// <summary>
        /// İade / işlem onaylandı
        /// </summary>
        [Display(Name = "Onaylandı")]
        Approved = 30,

        /// <summary>
        /// İade süreci tamamlandı
        /// </summary>
        [Display(Name = "Tamamlandı")]
        Completed = 40,

        /// <summary>
        /// Talep iptal edildi
        /// </summary>
        [Display(Name = "İptal Edildi")]
        Canceled = 50,

        /// <summary>
        /// Bilinmeyen / eşleşmeyen durum
        /// </summary>
        [Display(Name = "Bilinmiyor")]
        Unknown = 60
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
