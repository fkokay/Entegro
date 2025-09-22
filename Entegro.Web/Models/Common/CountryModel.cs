namespace Entegro.Web.Models.Common
{
    public class CountryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; } = true;
        public int DisplayOrder { get; set; }
        public List<CityModel> Cities { get; set; } = new List<CityModel>();
    }
}
