namespace Entegro.Application.DTOs.City
{
    public class CreateCityDto
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
    }
}
