using Entegro.Application.DTOs.Town;

namespace Entegro.Web.Models
{
    public class CityViewModel
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
        public List<TownDto>? Towns { get; set; }
    }
    public class ModalCityViewModel
    {
        public int CountryId { get; set; }
        public string CityName { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
    }
}
