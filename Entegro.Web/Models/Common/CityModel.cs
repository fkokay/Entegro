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
}
