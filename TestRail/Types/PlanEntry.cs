using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores information about a plan entry</summary>
    public class PlanEntry : BaseTestRailType
    {
        #region Public Properties
        /// <summary>Guid of the plan entry</summary>
        public string ID { get; set; }

        // TODO: Add summary
        public List<ulong> RunIDList { get; private set; }

        // TODO: Add summary
        public List<Run> RunList { get; set; }

        /// <summary>the id of the test suite for the test run</summary>
        public ulong? SuiteID { get; set; }

        /// <summary>name of the test run</summary>
        public string Name { get; set; }

        /// <summary>the ID of the user the test run should be assigned to</summary>
        public ulong? AssignedToID { get; set; }

        /// <summary>true for including all test cases of the test suite, false for a custom case selection</summary>
        public bool? IncludeAll { get; private set; }

        /// <summary>an array of case IDs for the custom case selection</summary>
        public List<ulong> CaseIDs { get; set; }

        /// <summary>an array of config IDs to allow for multiple test run configurations to be created</summary>
        public List<ulong> ConfigIDs { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>Parse a json object to a PlanEntry</summary>
        /// <param name="json">json object to parse</param>
        /// <returns>PlanEntry corresponding to a json object</returns>
        public static PlanEntry Parse(JObject json)
        {
            var pe = new PlanEntry
            {
                JsonFromResponse = json,
                ID = (string)json["id"],
                SuiteID = (ulong?)json["suite_id"],
                Name = (string)json["name"],
                AssignedToID = (ulong?)json["assignedto_id"],
                IncludeAll = (bool?)json["include_all"],
                RunIDList = _ConvertToRunIDs(json["runs"] as JArray),
                CaseIDs = _ConvertToCaseIDs(json["case_ids"] as JArray),
            };

            var jarray = json["runs"] as JArray;
            if (null != jarray)
            {
                pe.RunList = JsonUtility.ConvertJArrayToList(jarray, Run.Parse);
            }
            return pe;
        }

        /// <summary>Returns a json Object that represents this class</summary>
        /// <returns>Json object that corresponds to this class</returns>
        public JObject GetJson()
        {
            dynamic jsonParams = new JObject();
            if (null != SuiteID) { jsonParams.suite_id = SuiteID; }
            if (!string.IsNullOrWhiteSpace(Name)) { jsonParams.name = Name; }
            if (null != AssignedToID) { jsonParams.assignedto_id = AssignedToID.Value; }

            if (null != CaseIDs && 0 < CaseIDs.Count)
            {
                var jarray = new JArray();
                foreach (var caseID in CaseIDs)
                {
                    jarray.Add(caseID);
                }

                jsonParams.include_all = false;
                jsonParams.case_ids = jarray;
            }
            else
            {
                jsonParams.include_all = true;
            }

            if (null != ConfigIDs && 0 < ConfigIDs.Count)
            {
                var jarray = new JArray();
                foreach (var configID in ConfigIDs)
                {
                    jarray.Add(configID);
                }

                jsonParams.config_ids = jarray;
            }

            if (null != RunList && 0 < RunList.Count)
            {
                var jarray = new JArray();
                foreach (var run in RunList)
                {
                    jarray.Add(run.GetJson());
                }

                jsonParams.runs = jarray;
            }
            return jsonParams;
        }
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Convert the JArray to a list of Run IDs
        /// </summary>
        /// <param name="jarray">json to parse</param>
        /// <returns>a list of run IDs, list of size 0 if none exist</returns>
        private static List<ulong> _ConvertToRunIDs(JArray jarray)
        {
            var list = new List<ulong>();
            if (null != jarray)
            {
                list.AddRange(from jt in jarray where null != (ulong?) jt["id"] select (ulong) jt["id"]);
            }
            return list;
        }

        /// <summary>
        ///  Convert the Jarray to a list of case IDs
        /// </summary>
        /// <param name="jarray">json to parse</param>
        /// <returns>a list of case IDs, list of size 0 if none exist</returns>
        private static List<ulong> _ConvertToCaseIDs(JArray jarray)
        {
            var list = new List<ulong>();
            if (null != jarray)
            {
                list.AddRange(from JValue jsonItem in jarray select (ulong) jsonItem);
            }
            return list;
        }
        #endregion Private Methods
    }
}
