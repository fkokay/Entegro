namespace Entegro.Application.DTOs.Country
{
    public class UpdateCountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
    }
}
