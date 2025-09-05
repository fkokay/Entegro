
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    public class EmailAccountMap : IEntityTypeConfiguration<EmailAccount>
    {
        public void Configure(EntityTypeBuilder<EmailAccount> builder)
        {

        }
    }
    [Table("EmailAccount")]
    public class EmailAccount : BaseEntity
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public int SecureOption { get; set; }
        public bool UserDefaultCredentials { get; set; }
    }
}
