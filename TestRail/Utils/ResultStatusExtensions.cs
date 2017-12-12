namespace TestRail.Utils
{
    /// <summary>extension methods for the status enum</summary>
    public static class ResultStatusExtensions
    {
        /// <summary>gets the value of the enum as a string</summary>
        /// <param name="s">the status</param>
        /// <returns>the value of the status enum as a string</returns>
        public static string ValueAsString(this ResultStatus s)
        {
            return ((int)s).ToString();
        }
    }
}