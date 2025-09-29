namespace Entegro.Web.Models.Platform.Logging
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public string? Level { get; set; }
        public string? Message { get; set; }
        public string? MessageTemplate { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string? Exception { get; set; }
        public string? Properties { get; set; }
        public string? LogEvent { get; set; }
    }
}
