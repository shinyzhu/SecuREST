using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Pluxs.Securest.ApiWeb
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime Date1970 = new DateTime(1970, 1, 1);

        public static readonly string DefaultDateFormat = "yyyy-MM-dd";

        public static readonly string DefaultShortDateFormat = "M-d";

        public static readonly string DefaultTimeFormat = "HH:mm:ss";

        public static readonly string DefaultShortTimeFormat = "H:mm";

        public static readonly string DefaultDateTimeFormat = DefaultDateFormat + " " + DefaultTimeFormat;

        public static readonly string DefaultShortDateTimeFormat = DefaultShortDateFormat + " " + DefaultShortTimeFormat;

        public static string ToDateString(this DateTime? date)
        {
            return ToDateTimeString(date, DefaultDateFormat);
        }

        public static string ToDateString(this DateTime date)
        {
            return ToDateTimeString(date, DefaultDateFormat);
        }

        public static string ToTimeString(this DateTime? date)
        {
            return ToDateTimeString(date, DefaultTimeFormat);
        }

        public static string ToTimeString(this DateTime date)
        {
            return ToDateTimeString(date, DefaultTimeFormat);
        }

        public static string ToDateTimeString(this DateTime? date)
        {
            return ToDateTimeString(date, DefaultDateTimeFormat);
        }

        public static string ToDateTimeString(this DateTime? date, string format)
        {
            if (string.IsNullOrEmpty(format))
                format = DefaultDateTimeFormat;

            if (!date.HasValue)
                return string.Empty;

            return date.Value.ToString(format);
        }

        public static string ToDateTimeString(this DateTime date)
        {
            return ToDateTimeString(date, DefaultDateTimeFormat);
        }

        public static string ToDateTimeString(this DateTime date, string format)
        {
            if (string.IsNullOrEmpty(format))
                format = DefaultDateTimeFormat;

            return date.ToString(format);
        }

        public static bool IsToday(this DateTime? date)
        {
            if (!date.HasValue)
                return false;

            return date.Value.Date == DateTime.Today;
        }

        public static string ToWeekDayName(this DateTime? date)
        {
            if (!date.HasValue)
                return string.Empty;

            return ToWeekDayName(date.Value);
        }

        public static string ToWeekDayName(this DateTime date)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek);
        }


        /// <summary>
        /// Unix epoch seconds
        /// </summary>
        /// <param name="date"></param>
        /// <seealso cref="http://en.wikipedia.org/wiki/Unix_epoch"/>
        /// <returns></returns>
        public static double ToUnixSeconds(this DateTime date)
        {
            var utcDate = date.ToUniversalTime();
            var spanFrom1970 = new TimeSpan(utcDate.Ticks - Date1970.Ticks);

            return spanFrom1970.TotalSeconds;
        }
    }
}