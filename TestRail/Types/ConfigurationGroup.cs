using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TestRail.Utils;

namespace TestRail.Types
{
    /// <summary>Stores information about a configuration group in TestRail.</summary>
    public class ConfigurationGroup : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the current configuration group.</summary>
        public ulong Id { get; set; }

        /// <summary>Project id associated with the current configuration group.</summary>
        public ulong ProjectId { get; set; }

        /// <summary>Name of the current configuration group.</summary>
        public string Name { get; set; }

        /// <summary>List of configurations associated with the current configuration group.</summary>
        public List<Configuration> Configurations { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>Parses JSON into a ConfigurationGroup.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>ConfigurationGroup corresponding to the JSON.</returns>
        public static ConfigurationGroup Parse(JObject json)
        {
            var configurationGroup = new ConfigurationGroup
            {
                JsonFromResponse = json,
                Id = (ulong)json["id"],
                Name = (string)json["name"],
                ProjectId = (ulong)json["project_id"]
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
