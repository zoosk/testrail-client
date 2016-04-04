using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    public class Configuration : BaseTestRailType
    {
        #region Public Properties
        // TODO: Add summary
        public ulong ID { get; set; }

        // TODO: Add summary
        public ulong ConfigurationGroupID { get; set; }

        // TODO: Add summary
        public string Name { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>parses json into a Configuration</summary>
        /// <param name="json">json to parse</param>
        /// <returns>Configuration corresponding to the json</returns>
        public static Configuration Parse(JObject json)
        {
            var configuration = new Configuration
            {
                JsonFromResponse = json,
                ID = (ulong)json["id"],
                Name = (string)json["name"],
                ConfigurationGroupID = (ulong)json["group_id"],
            };
            return configuration;
        }
        #endregion Public Methods
    }
}
