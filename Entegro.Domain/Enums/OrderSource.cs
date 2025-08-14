using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Enums
{
    public enum OrderSource
    {
        [Description("Smartstore")]
        Smartstore = 1,
        [Description("Trendyol")]
        Trendyol = 2,
    }
}
