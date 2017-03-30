using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    public abstract class BaseTestRailType
    {
        /// <summary>
        /// Raw JSON received from API
        /// </summary>
        protected JObject JsonFromResponse { get; set; }

        public string GetCustomField(string fieldName)
        {
            return (string)JsonFromResponse[fieldName];
        }
    }
}
