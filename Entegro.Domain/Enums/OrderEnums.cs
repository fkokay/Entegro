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
        [Description("Hepsiburada")]
        Hepsiburada = 3,
    }
    public enum OrderStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Processing
        /// </summary>
        Processing = 20,

        /// <summary>
        /// Complete
        /// </summary>
        Complete = 30,

        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 40
    }
    public enum ReturnRequestStatus
    {
        /// <summary>
        /// Pending.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Received.
        /// </summary>
        Received = 10,

        /// <summary>
        /// Return authorized.
        /// </summary>
        ReturnAuthorized = 20,

        /// <summary>
        /// Item(s) repaired.
        /// </summary>
        ItemsRepaired = 30,

        /// <summary>
        /// Item(s) refunded.
        /// </summary>
        ItemsRefunded = 40,

        /// <summary>
        /// Request rejected.
        /// </summary>
        RequestRejected = 50,

        /// <summary>
        /// Cancelled.
        /// </summary>
        Cancelled = 60
    }
}
