using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.EInvoice.EFinans
{
    [JsonObject]
    public class EFinansInput
    {
        [JsonProperty("paketOid")]
        public string PaketOid { get; set; }

        [JsonProperty("orjinalBelgeFormati")]
        public string OrjinalBelgeFormati { get; set; }

        [JsonProperty("paketDosyaOid")]
        public string PaketDosyaOid { get; set; }

        [JsonProperty("imzaliBelgePath")]
        public string ImzaliBelgePath { get; set; }

        [JsonProperty("wsKontrolFaturaDurum")]
        public string WsKontrolFaturaDurum { get; set; }

        [JsonProperty("faturaUuid")]
        public string FaturaUuid { get; set; }

        [JsonProperty("itirazBelgeTarih")]
        public DateTime? ItirazBelgeTarih { get; set; }

        [JsonProperty("faturaNo")]
        public string FaturaNo { get; set; }

        [JsonProperty("itirazTarihi")]
        public DateTime? ItirazTarihi { get; set; }

        [JsonProperty("faturaSeri")]
        public string FaturaSeri { get; set; }

        [JsonProperty("kaynak")]
        public string Kaynak { get; set; }

        [JsonProperty("itirazYontemi")]
        public string ItirazYontemi { get; set; }

        [JsonProperty("vkn")]
        public string Vkn { get; set; }

        [JsonProperty("paketOnayDurumu")]
        public string PaketOnayDurumu { get; set; }

        [JsonProperty("paketAdi")]
        public string PaketAdi { get; set; }

        [JsonProperty("paketDosyaAdi")]
        public string PaketDosyaAdi { get; set; }

        [JsonProperty("aliciEposta")]
        public string AliciEposta { get; set; }

        [JsonProperty("kasa")]
        public string Kasa { get; set; }

        [JsonProperty("islemId")]
        public string IslemId { get; set; }

        [JsonProperty("wsKontrolSonUpdateZamani")]
        public DateTime? WsKontrolSonUpdateZamani { get; set; }

        [JsonProperty("iadeFaturaTarihi")]
        public DateTime? IadeFaturaTarihi { get; set; }

        [JsonProperty("goruntuOlusturulsunMu")]
        public bool GoruntuOlusturulsunMu { get; set; }

        [JsonProperty("kullaniciKodu")]
        public string KullaniciKodu { get; set; }

        [JsonProperty("sablonAdi")]
        public string SablonAdi { get; set; }

        [JsonProperty("faturaSiraNo")]
        public string FaturaSiraNo { get; set; }

        [JsonProperty("donenBelgeFormati")]
        public string DonenBelgeFormati { get; set; }

        [JsonProperty("erpKodu")]
        public string ErpKodu { get; set; }

        [JsonProperty("ozetDeger")]
        public string OzetDeger { get; set; }

        [JsonProperty("itirazAciklama")]
        public string ItirazAciklama { get; set; }

        [JsonProperty("taslagaYonlendir")]
        public bool TaslagaYonlendir { get; set; }

        [JsonProperty("versiyon")]
        public string Versiyon { get; set; }

        [JsonProperty("faturaEpostaOid")]
        public string FaturaEpostaOid { get; set; }

        [JsonProperty("iadeFaturaNo")]
        public string IadeFaturaNo { get; set; }

        [JsonProperty("kontorDurum")]
        public string KontorDurum { get; set; }

        [JsonProperty("sube")]
        public string Sube { get; set; }

        [JsonProperty("islemIp")]
        public string IslemIp { get; set; }

        [JsonProperty("inputHash")]
        public string InputHash { get; set; }

        [JsonProperty("itirazBelgeNo")]
        public string ItirazBelgeNo { get; set; }

        [JsonProperty("numaraVerilsinMi")]
        public bool NumaraVerilsinMi { get; set; }

        [JsonProperty("faturaImzalansinMi")]
        public bool FaturaImzalansinMi { get; set; }

        [JsonProperty("ublCustomizationVersion")]
        public string UblCustomizationVersion { get; set; }

        [JsonProperty("faturaTipi")]
        public string FaturaTipi { get; set; }

        [JsonProperty("tarih")]
        public DateTime? Tarih { get; set; }

        [JsonProperty("yerelFaturaNo")]
        public string YerelFaturaNo { get; set; }

        [JsonProperty("sessionKontrolEdilsinmi")]
        public bool SessionKontrolEdilsinmi { get; set; }
    }
}
