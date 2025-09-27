using Entegro.Data.Caching;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Platform.Logging
{
    /// <summary>
    /// Represents an activity log type record
    /// </summary>
    [CacheableEntity]
    [Table("ActivityLogType")]
    public partial class ActivityLogType : BaseEntity
    {
        /// <summary>
        /// Gets or sets the system keyword
        /// </summary>
        [Required, StringLength(100)]
        public string SystemKeyword { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        [Required, StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the activity log type is enabled
        /// </summary>
        public bool Enabled { get; set; }

    }
}
