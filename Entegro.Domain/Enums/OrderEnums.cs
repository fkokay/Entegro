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
        /// Müşteri iade talebi oluşturdu
        /// Trendyol: Created
        /// </summary>
        [Display(Name = "Beklemede")]
        Created = 0,


        ////1 hepsini getir



        /// <summary>
        /// Ürün satıcıya ulaştı, aksiyon bekleniyor
        /// Trendyol: WaitingInAction
        /// </summary>
        [Display(Name = "Aksiyon Bekleniyor")]
        WaitingInAction = 2,

        /// <summary>
        /// İade onaylandı, fraud kontrolünde
        /// Trendyol: WaitingFraudCheck
        /// </summary>
        [Display(Name = "Fraud Kontrolünde")]
        WaitingFraudCheck = 3,

        /// <summary>
        /// Analiz sürecinde
        /// Trendyol: InAnalysis
        /// </summary>
        [Display(Name = "Analizde")]
        InAnalysis = 4,

        /// <summary>
        /// İhtilaflı iade
        /// Trendyol: Unresolved
        /// </summary>
        [Display(Name = "İhtilaflı")]
        Unresolved = 5,

        /// <summary>
        /// Satıcı tarafından kabul edildi
        /// Trendyol: Accepted
        /// </summary>
        [Display(Name = "Kabul Edildi")]
        Accepted = 6,

        /// <summary>
        /// Satıcı tarafından reddedildi
        /// Trendyol: Rejected
        /// </summary>
        [Display(Name = "Reddedildi")]
        Rejected = 7,

        /// <summary>
        /// İade iptal edildi
        /// Trendyol: Cancelled
        /// </summary>
        [Display(Name = "İptal Edildi")]
        Cancelled = 8,

        /// <summary>
        /// Tanımsız / beklenmeyen durum
        /// </summary>
        [Display(Name = "Bilinmiyor")]
        Unknown = 99
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
