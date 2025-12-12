namespace Entegro.Application.DTOs.ArasQueryResult
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class QueryResult
    {

        private QueryResultCollection collectionField;

        /// <remarks/>
        public QueryResultCollection Collection
        {
            get
            {
                return this.collectionField;
            }
            set
            {
                this.collectionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QueryResultCollection
    {
        public string MUSTERI_OZEL_KODU { get; set; }
        public string IRSALIYE_NUMARA { get; set; }
        public string GONDERICI { get; set; }
        public string ALICI { get; set; }
        public string KARGO_LINK_NO { get; set; }
        public string KARGO_TAKIP_NO { get; set; }
        public string CIKIS_SUBE { get; set; }
        public string VARIS_SUBE { get; set; }
        public System.DateTime CIKIS_TARIH { get; set; }
        public int ADET { get; set; }
        public decimal DESI { get; set; }
        public string ODEME_TIPI { get; set; }
        public decimal TUTAR { get; set; }
        public string REFERANS { get; set; }
        public string? TESLIM_ALAN { get; set; }
        public string? TESLIM_TARIHI { get; set; }
        public string? TESLIM_SAATI { get; set; }
        public string VARIS_KODU { get; set; }
        public int TIP_KODU { get; set; }
        public int DURUM_KODU { get; set; }
        public string DURUMU { get; set; }
        public string? IADE_SEBEBI { get; set; }
        public int WORLDWIDE { get; set; }
        public string KARGO_KODU { get; set; }
        public string DURUM_EN { get; set; }
        public decimal TAHSILAT_TUTARI { get; set; }
        public string TAHSILAT_TIPI { get; set; }
        public decimal ODEME_TUTARI { get; set; }
        public DateTime ODEME_TARIHI { get; set; }
        public int TAHSILAT_IPTAL { get; set; }
    }
}
