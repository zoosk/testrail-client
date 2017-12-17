using TestRail.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TestRail.Types
{
    /// <inheritdoc />
    /// <summary>
    /// TODO - Add summary
    /// </summary>
    public class ConfigurationGroup : BaseTestRailType
    {
        #region Public Properties
        /// <summary>
        /// TODO - Add summary
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// TODO - Add summary
        /// </summary>
        public ulong ProjectId { get; set; }

        /// <summary>
        /// TODO - Add summary
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// TODO - Add summary
        /// </summary>
        public List<Configuration> Configurations { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>parses json into a ConfigurationGroup</summary>
        /// <param name="json">json to parse</param>
        /// <returns>ConfigurationGroup corresponding to the json</returns>
        public static ConfigurationGroup Parse(JObject json)
        {
            var configurationGroup = new ConfigurationGroup
            {
                JsonFromResponse = json,
                Id = (ulong)json["id"],
                Name = (string)json["name"],
                ProjectId = (ulong)json["project_id"],
            };

            var jarray = json["configs"] as JArray;

            if (null != jarray)
                configurationGroup.Configurations = JsonUtility.ConvertJArrayToList(jarray, Configuration.Parse);

            return configurationGroup;
        }
        #endregion Public Methods
    }
}
