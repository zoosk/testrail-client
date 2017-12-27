using System;
using System.Text.RegularExpressions;

namespace TestRail.Utils
{
    /// <summary>TimeSpan Utility methods</summary>
    internal static class TimeSpanUtility
    {
        /// <summary>Convert string timespan into a TimeSpan object</summary>
        /// <param name="value">string to convert</param>
        /// <returns>TimeSpan if conversion was good, null otherwise</returns>
        internal static TimeSpan? FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            TimeSpan? retVal;
            var days = 0;
            var hours = 0;
            var minutes = 0;
            var seconds = 0;
            var regex = new Regex(@"(\d+)([a-zA-Z]+)");
            var timesplits = value.Split(' ');

            foreach (var timesplit in timesplits)
            {
                var result = regex.Match(timesplit);
                var numeric = int.Parse(result.Groups[1].Value);
                var alpha = result.Groups[2].Value;

                alpha = alpha.ToLower();

                if (alpha.StartsWith("d"))
                {
                    days = numeric;
                }

                else if (alpha.StartsWith("h"))
                {
                    hours = numeric;
                }

                else if (alpha.StartsWith("m"))
                {
                    minutes = numeric;
                }

                else if (alpha.StartsWith("s"))
                {
                    seconds = numeric;
                }

                else
                {
                    throw new FormatException("Unknown time component");
                }
            }

            retVal = new TimeSpan(days, hours, minutes, seconds);

            return retVal;
        }
    }
}
