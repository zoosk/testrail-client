using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <inheritdoc />
    /// <summary>stores information about a projects</summary>
    public class Projects : BaseTestRailType
    {
        #region Public Properties
        /// <summary>Where to start counting the projects from</summary>
        [JsonProperty("offset", Required = Required.Always)]
        public ulong Offset { get; set; }

        /// <summary>The number of projects the response should return</summary>
        [JsonProperty("limit", Required = Required.Always)]
        public ulong Limit { get; set; }

        /// <summary>The size of the returned list of projects</summary>
        [JsonProperty("size", Required = Required.Always)]
        public ulong Size { get; set; }

        /// <summary>Next group of results for the pagination</summary>
        [JsonProperty("_links", Required = Required.Always)]
        public Links Links { get; set; }

        /// <summary>The list of returned projects</summary>
        [JsonProperty("projects", Required = Required.Always)]
        public List<Project> ProjectsList { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>parses json into a projects</summary>
        /// <param name="json">json to parse</param>
        /// <returns>projects corresponding to the json</returns>
        public static Projects Parse(JObject json)
        {
            var projects = new Projects
            {
                JsonFromResponse = json,
                Offset = (ulong)json["offset"],
                Limit = (ulong)json["limit"],
                Size = (ulong)json["size"],
                Links = JsonConvert.DeserializeObject<Links>(json["_links"].ToString()),
                ProjectsList = JsonConvert.DeserializeObject<List<Project>>(json["projects"].ToString())
            };

            return projects;
        }
        #endregion Public Methods
    }
}
