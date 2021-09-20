using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TestRail.Types
{
    /// <inheritdoc />
    /// <summary>Link object for moving through groups of returned items based on pagination</summary>
    public class Links
    {
        /// <summary>
        /// The link to the next group of returned projects for the pagination
        /// </summary>
        [JsonProperty("next", Required = Required.AllowNull)]
        public object Next { get; set; }

        /// <summary>
        /// The link to the previous group of returned projects for the pagination
        /// </summary>
        [JsonProperty("prev", Required = Required.AllowNull)]
        public object Prev { get; set; }
    }
}
