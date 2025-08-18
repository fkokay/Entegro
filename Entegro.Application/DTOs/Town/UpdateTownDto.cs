namespace Entegro.Application.DTOs.Town
{
    public class UpdateTownDto
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }

    }
}
