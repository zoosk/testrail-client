using System;
using System.Text.RegularExpressions;

namespace TestRail
{
    /// <summary>
    /// TimeSpan Utility methods
    /// </summary>
    internal static class TimeSpanUtility
    {
        /// <summary>
        /// Convert string timespan into a TimeSpan object
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>TimeSpan if conversion was good, null otherwise</returns>
        internal static TimeSpan? FromString(String value)
        {
            TimeSpan? retVal = null;
            if (!string.IsNullOrWhiteSpace(value))
            {
                int days = 0;
                int hours = 0;
                int minutes = 0;
                int seconds = 0;

                Regex re = new Regex(@"(\d+)([a-zA-Z]+)");
                var timesplits = value.Split(' ');

                foreach (var timesplit in timesplits)
                {
                    Match result = re.Match(timesplit);
                    int numeric = int.Parse(result.Groups[1].Value);
                    string alpha = result.Groups[2].Value;
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
            }
            return retVal;
        }

    }
}

