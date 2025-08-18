namespace Entegro.Application.DTOs.District
{
    public class UpdateDistrictDto
    {
        public int Id { get; set; }
        public int TownId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }
    }
}
