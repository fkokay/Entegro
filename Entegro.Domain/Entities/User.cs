using Entegro.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    [Table("User")]
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; } = true;
    }
}
