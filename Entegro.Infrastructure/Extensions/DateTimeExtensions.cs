using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToTimeAgo(this DateTime dateTime)
        {
            var ts = DateTime.Now - dateTime;

            if (ts.TotalSeconds < 60)
                return $"{ts.Seconds} saniye önce";
            if (ts.TotalMinutes < 60)
                return $"{ts.Minutes} dakika önce";
            if (ts.TotalHours < 24)
                return $"{ts.Hours} saat önce";
            if (ts.TotalDays < 30)
                return $"{ts.Days} gün önce";
            if (ts.TotalDays < 365)
                return $"{(int)(ts.TotalDays / 30)} ay önce";

            return $"{(int)(ts.TotalDays / 365)} yıl önce";
        }
    }
}
