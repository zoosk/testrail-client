using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>Stores details about a configuration in TestRail.</summary>
    public class Configuration : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the current configuration.</summary>
        public ulong Id { get; set; }

        /// <summary>group id of the current configuration.</summary>
        public ulong ConfigurationGroupId { get; set; }

        /// <summary>name of the current configuration.</summary>
        public string Name { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>Parses JSON into a Configuration.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Configuration corresponding to the JSON.</returns>
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
