using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    public class Configuration
    {
        public ulong ID { get; set; }
        public ulong ConfigurationGroupID { get; set; }
        public string Name { get; set; }

        /// <summary>parses json into a Configuration</summary>
        /// <param name="json">json to parse</param>
        /// <returns>Configuration corresponding to the json</returns>
        public static Configuration Parse(JObject json)
        {
            Configuration configuration = new Configuration();
            configuration.ID = (ulong)json["id"];
            configuration.Name = (string)json["name"];
            configuration.ConfigurationGroupID = (ulong)json["group_id"];
            return configuration;
        }
    }
}
