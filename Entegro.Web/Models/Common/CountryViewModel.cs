namespace Entegro.Web.Models.Common
{
    public class CountryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; } = true;
        public int DisplayOrder { get; set; }
        public List<CityViewModel> Cities { get; set; } = new List<CityViewModel>();
    }
}
