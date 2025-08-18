namespace Entegro.Application.DTOs.City
{
    public class UpdateCityDto
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
    }
}
