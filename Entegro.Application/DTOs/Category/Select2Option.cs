namespace Entegro.Application.DTOs.Category
{
    public sealed class Select2OptionDto
    {
        public int id { get; set; }
        public string text { get; set; } = "";
    }
    public sealed class Select2ResponseDto
    {
        public List<Select2OptionDto> results { get; set; } = new();
        public Pagination pagination { get; set; } = new();
        public sealed class Pagination { public bool more { get; set; } }
    }
}
