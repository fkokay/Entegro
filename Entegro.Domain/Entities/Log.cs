using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("Log")]
    public class Log :BaseEntity
    {
        public int LogLevelId { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Logger { get; set; }
    }
}
