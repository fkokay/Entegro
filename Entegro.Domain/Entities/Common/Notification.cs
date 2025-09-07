using Entegro.Domain.Entities.Platform.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Common
{
    [Table("Notifications")]
    public class Notification :BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsRead { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
