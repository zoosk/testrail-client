using System;

namespace TestRail.Utils
{
    /// <summary>Extension methods for the <see cref="DateTime"/> class.</summary>
    public static class DateTimeExtensions
    {
        /// <summary>Converts the date to a UNIX timestamp.</summary>
        /// <returns>A UNIX time stamp representing the supplied <see cref="DateTime"/>.</returns>
        public static double ToUnixTimestamp(this DateTime dt)
        {
            return dt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>Generates a <see cref="DateTime"/> from a UNIX timestamp.</summary>
        /// <param name="timestamp">A UNIX timestamp.</param>
        /// <returns><see cref="DateTime"/> corresponding to the supplied UNIX timestamp.</returns>
        public static DateTime FromUnixTimeStamp(double timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp).ToLocalTime();
        }
    }
}
