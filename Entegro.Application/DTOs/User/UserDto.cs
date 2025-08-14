namespace Entegro.Application.DTOs.User
{
    public class UserDto
    {
        public string FirsName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; } = true;
    }
}
