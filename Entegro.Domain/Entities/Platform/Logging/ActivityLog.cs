using Entegro.Data.Caching;
using Entegro.Data.Hooks;
using Entegro.Domain.Entities.Checkout;
using Entegro.Domain.Entities.Platform.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Entegro.Domain.Entities.Platform.Logging
{
    public class ActivityLogMap : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLog");
            builder
                .HasOne(c => c.ActivityLogType)
                .WithMany()
                .HasForeignKey(c => c.ActivityLogTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    /// <summary>
    /// Represents an activity log record
    /// </summary>
    [Hookable(false)]
    [Index(nameof(CreatedOnUtc), Name = "IX_ActivityLog_CreatedOnUtc")]
    [CacheableEntity(NeverCache = true)]
    public partial class ActivityLog : BaseEntity
    {
        /// <summary>
        /// Gets or sets the activity log type identifier
        /// </summary>
        public int ActivityLogTypeId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the activity comment
        /// </summary>
        [Required, MaxLength()]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the activity log type
        /// </summary>
        public virtual ActivityLogType ActivityLogType { get; set; }
        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual User User { get; set; }
    }
}
