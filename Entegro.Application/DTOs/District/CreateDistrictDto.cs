namespace Entegro.Application.DTOs.District
{
    public class CreateDistrictDto
    {
        public int TownId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
    }
}
