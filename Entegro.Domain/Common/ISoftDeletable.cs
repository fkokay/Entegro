using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Common
{
    public interface ISoftDeletable
    {
        bool Deleted { get; set; }

        [NotMapped, IgnoreDataMember]
        bool ForceDeletion
        {
            get { return false; }
        }
    }
}
