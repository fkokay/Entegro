namespace Entegro.Web.Models
{
    public class EmailAccountViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public int SecureOption { get; set; }
        public bool UserDefaultCredentials { get; set; }
    }
}
