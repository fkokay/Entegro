namespace Entegro.Application.DTOs.Marketplace.Pazarama
{
    public class PazaramaApiContext
    {
        public string BaseUrlToken = "https://isortagimgiris.pazarama.com/";
        public string BaseUrl = "https://isortagimapi.pazarama.com/";
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
