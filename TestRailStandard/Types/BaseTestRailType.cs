using Newtonsoft.Json.Linq;

namespace TestRailStandard.Types
{
    public abstract class BaseTestRailType
    {
        /// <summary>
        /// Raw JSON received from API
        /// </summary>
        public JObject JsonFromResponse { get; set; }
    }
}
