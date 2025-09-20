using Entegro.Application.DTOs.Town;

namespace Entegro.Web.Models.Common
{
    public class CityModel
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
        public List<TownDto>? Towns { get; set; }
    }
    public class ModalCityModel
    {
        public int CountryId { get; set; }
        public string CityName { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
    }

    public class ModalTownModel
    {
        public int CityId { get; set; }
        public string TownName { get; set; }
        public int TownDisplayOrder { get; set; }
        public bool Published { get; set; }
    }
}
