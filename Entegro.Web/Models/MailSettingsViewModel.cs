namespace Entegro.Web.Models
{
    public class MailSettingsViewModel
    {
        public string Email { get; set; } = null!;
        public string MailType { get; set; }
        public string EmailDisplayName { get; set; } = null!;
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 25;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string EncryptionType { get; set; } = null!;
        public int IntegrationSystemId { get; set; }
    }
}
