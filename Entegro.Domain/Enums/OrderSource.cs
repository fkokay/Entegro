using System.ComponentModel;

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
