using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores informations about the context for a case field's config section</summary>
    public class ConfigContext : BaseTestRailType
    {
        #region Public Properties
        /// <summary>Is the context global</summary>
        public bool? IsGlobal { get; private set; }

        /// <summary>List of project IDs</summary>
        public List<string> ProjectIDs { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>parse a json object into a Config Context</summary>
        /// <param name="json">takes a json object and converts it to a ConfigContext</param>
        public static ConfigContext Parse(JObject json)
        {
            var cc = new ConfigContext
            {
                JsonFromResponse = json,
                IsGlobal = (bool?)json["is_global"],
            };

            // check to see if the project ids is empty 
            var jval = json["project_ids"];
            if (null != jval && jval.HasValues)
            {
                // add values to the list if not empty
                cc.ProjectIDs = new List<string>();
                var jarray = (JArray)jval;
                foreach (var jsonItem in jarray.Cast<JValue>())
                {
                    cc.ProjectIDs.Add((string)jsonItem);
                }
            }
            return cc;
        }
        #endregion Public Methods
    }
}
