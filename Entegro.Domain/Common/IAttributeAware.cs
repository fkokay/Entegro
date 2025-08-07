using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Common
{
    public partial interface IAttributeAware
    {
        public string RawAttributes { get; set; }
    }
}
