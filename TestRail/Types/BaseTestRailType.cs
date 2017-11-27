using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    public abstract class BaseTestRailType
    {
        /// <summary>
        /// Raw JSON received from API
        /// </summary>
        public JObject JsonFromResponse { get; set; }
    }
}
