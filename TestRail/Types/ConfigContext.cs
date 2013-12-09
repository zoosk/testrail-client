using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TestRail.Types
{
    /// <summary>stores informations about the context for a case field's config section</summary>
    public class ConfigContext
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
            ConfigContext cc = new ConfigContext();
            cc.IsGlobal = (bool?)json["is_global"];

            // check to see if the project ids is empty 
            JToken jval = json["project_ids"];
            if (null != jval && jval.HasValues)
            {
                // add values to the list if not empty
                cc.ProjectIDs = new List<string>();
                JArray jarray = (JArray)jval;
                foreach (JValue jsonItem in jarray)
                {
                    cc.ProjectIDs.Add((string)jsonItem);
                }
            }
            return cc;
        }
        #endregion
    }
}
