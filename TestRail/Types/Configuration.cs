using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <inheritdoc />
    /// <summary>
    /// TODO - Add summary
    /// </summary>
    public class Configuration : BaseTestRailType
    {
        #region Public Properties
        /// <summary>
        /// TODO - Add summary
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// TODO - Add summary
        /// </summary>
        public ulong ConfigurationGroupId { get; set; }

        /// <summary>
        /// TODO - Add summary
        /// </summary>
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
                Id = (ulong)json["id"],
                Name = (string)json["name"],
                ConfigurationGroupId = (ulong)json["group_id"]
            };

            return configuration;
        }
        #endregion Public Methods
    }
}
