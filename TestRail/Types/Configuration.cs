using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    public class Configuration : BaseTestRailType
    {
        public ulong ID { get; set; }

        public ulong ConfigurationGroupID { get; set; }

        public string Name { get; set; }

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
    }
}
