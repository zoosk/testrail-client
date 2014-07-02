using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    public class ConfigurationGroup
    {
        public ulong ID { get; set; }
        public ulong ProjectID { get; set; }
        public string Name { get; set; }
        public List<Configuration> Configurations { get; set; }

        /// <summary>parses json into a ConfigurationGroup</summary>
        /// <param name="json">json to parse</param>
        /// <returns>ConfigurationGroup corresponding to the json</returns>
        public static ConfigurationGroup Parse(JObject json)
        {
            ConfigurationGroup configurationGroup = new ConfigurationGroup();
            configurationGroup.ID = (ulong)json["id"];
            configurationGroup.Name = (string)json["name"];
            configurationGroup.ProjectID = (ulong)json["project_id"];

            JArray jarray = json["configs"] as JArray;
            if (null != jarray)
            {
                configurationGroup.Configurations = JsonUtility.ConvertJArrayToList<Configuration>(jarray, Configuration.Parse);
            }
            return configurationGroup;
        }        
    }
}
