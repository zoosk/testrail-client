using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TestRail.Types
{
    /// <inheritdoc />
    /// <summary>stores information about a projects</summary>
    public class Suites : BaseTestRailType
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
        [JsonProperty("suites", Required = Required.Always)]
        public List<Suite> SuitesList { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>parses json into a suites</summary>
        /// <param name="json">json to parse</param>
        /// <returns>suites corresponding to the json</returns>
        public static Suites Parse(JObject json)
        {
            var suites = new Suites
            {
                JsonFromResponse = json,
                Offset = (ulong)json["offset"],
                Limit = (ulong)json["limit"],
                Size = (ulong)json["size"],
                Links = JsonConvert.DeserializeObject<Links>(json["_links"].ToString()),
                SuitesList = JsonConvert.DeserializeObject<List<Suite>>(json["suites"].ToString())
            };

            return suites;
        }
        #endregion Public Methods
    }
}
