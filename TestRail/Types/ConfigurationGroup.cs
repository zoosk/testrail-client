using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    public class ConfigurationGroup : BaseTestRailType
    {
        #region Public Properties
        // TODO: Add summary
        public ulong ID { get; set; }

        // TODO: Add summary
        public ulong ProjectID { get; set; }

        // TODO: Add summary
        public string Name { get; set; }

        // TODO: Add summary
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
                ID = (ulong)json["id"],
                Name = (string)json["name"],
                ProjectID = (ulong)json["project_id"],
            };

            var jarray = json["configs"] as JArray;
            if (null != jarray)
            {
                configurationGroup.Configurations = JsonUtility.ConvertJArrayToList(jarray, Configuration.Parse);
            }
            return configurationGroup;
        }
        #endregion Public Methods
    }
}
