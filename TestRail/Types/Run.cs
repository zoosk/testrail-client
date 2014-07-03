using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TestRail.Types
{
    /// <summary>stores information about a run</summary>
    public class Run
    {
        #region Properties
        /// <summary>id of the run</summary>
        public ulong? ID { get; private set; }

        /// <summary>name of the run</summary>
        public string Name { get; set; }

        /// <summary>description of the run</summary>
        public string Description { get; set; }

        /// <summary>id of the suite associated with the run</summary>
        public ulong? SuiteID { get; set; }

        /// <summary>id of the milestone associated with the run</summary>
        public ulong? MilestoneID { get; set; }

        /// <summary>config for the run</summary>
        public string Config { get; private set; }

        /// <summary>true if the run has been completes</summary>
        public bool? IsCompleted { get; private set; }

        /// <summary>date on which the run which was completed</summary>
        public DateTime? CompletedOn { get; private set; }

        /// <summary>number of tests in the plan that passed</summary>
        public uint? PassedCount { get; private set; }

        /// <summary>number of tests in the plan that are blocked</summary>
        public uint? BlockedCount { get; private set; }

        /// <summary>number of tests in the plan that are untested</summary>
        public uint? UntestedCount { get; private set; }

        /// <summary>number of tests in the plan that need to be retested</summary>
        public uint? RetestCount { get; private set; }

        /// <summary>number of tests in the plan that failed</summary>
        public uint? FailedCount { get; private set; }

        /// <summary>id of the project associated with the run</summary>
        public ulong? ProjectID { get; private set; }

        /// <summary>id of the plan associated with the run</summary>
        public ulong? PlanID { get; private set; }

        /// <summary>is of the user it is assigned to</summary>
        public ulong? AssignedTo { get; set; }

        /// <summary></summary>
        public bool IncludeAll { get; set; }

        /// <summary></summary>
        public ulong CustomStatus1Count { get; private set; }

        /// <summary></summary>
        public ulong CustomStatus2Count { get; private set; }

        /// <summary></summary>
        public ulong CustomStatus3Count { get; private set; }

        /// <summary></summary>
        public ulong CustomStatus4Count { get; private set; }

        /// <summary></summary>
        public ulong CustomStatus5Count { get; private set; }

        /// <summary></summary>
        public ulong CustomStatus6Count { get; private set; }

        /// <summary></summary>
        public ulong CustomStatus7Count { get; private set; }

        /// <summary></summary>
        public string Url { get; private set; }

        /// <summary>an array of case IDs for the custom case selection</summary>
        public HashSet<ulong> CaseIDs { get; set; }

        /// <summary>an array of case IDs for the custom case selection</summary>
        public List<ulong> ConfigIDs { get; set; }
        #endregion Properties

        #region Public Methods
        /// <summary>string representation of the object</summary>
        /// <returns>string representation of the object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>parses json into a run</summary>
        /// <param name="json">json to parse</param>
        /// <returns>run corresponding to the json</returns>
        public static Run Parse(JObject json)
        {
            Run r = new Run();
            r.ID = (ulong?)json["id"];
            r.Name = (string)json["name"];
            r.Description = (string)json["description"];
            r.SuiteID = (ulong?)json["suite_id"];
            r.MilestoneID = (ulong?)json["milestone_id"];
            r.Config = (string)json["config"];
            r.IsCompleted = (bool?)json["is_completed"];
            r.CompletedOn = ((null == (int?)json["completed_on"]) ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["completed_on"]));
            r.PassedCount = (uint?)json["passed_count"];
            r.BlockedCount = (uint?)json["blocked_count"];
            r.UntestedCount = (uint?)json["untested_count"];
            r.RetestCount = (uint?)json["retest_count"];
            r.FailedCount = (uint?)json["failed_count"];
            r.ProjectID = (ulong?)json["project_id"];
            r.PlanID = (ulong?)json["plan_id"];
            r.CustomStatus1Count = (ulong)json["custom_status1_count"];
            r.CustomStatus2Count = (ulong)json["custom_status2_count"];
            r.CustomStatus3Count = (ulong)json["custom_status3_count"];
            r.CustomStatus4Count = (ulong)json["custom_status4_count"];
            r.CustomStatus5Count = (ulong)json["custom_status5_count"];
            r.CustomStatus6Count = (ulong)json["custom_status6_count"];
            r.CustomStatus7Count = (ulong)json["custom_status7_count"];
            r.AssignedTo = (ulong?)json["assignedto_id"];
            r.IncludeAll = (bool)json["include_all"];
            r.Url = (string)json["url"];
            return r;
        }

        /// <summary>Creates a json object for this class</summary>
        /// <returns>json object that represents this class</returns>
        public JObject GetJson()
        {
            dynamic jsonParams = new JObject();
            if (null != SuiteID) { jsonParams.suite_id = SuiteID; }
            if (!string.IsNullOrWhiteSpace(Name)) { jsonParams.name = Name; }
            if (null != Description) { jsonParams.description = Description; }
            if (null != MilestoneID) { jsonParams.milestone_id = MilestoneID; }
            if (null != AssignedTo) { jsonParams.assignedto_id = AssignedTo; }
            jsonParams.include_all = IncludeAll;

            if (null != CaseIDs && 0 < CaseIDs.Count)
            {
                JArray jarray = new JArray();
                foreach (ulong caseID in CaseIDs)
                {
                    jarray.Add(caseID);
                }
                jsonParams.case_ids = jarray;
            }

            if (null != ConfigIDs && 0 < ConfigIDs.Count)
            {
                JArray jarray = new JArray();
                foreach (ulong configID in ConfigIDs)
                {
                    jarray.Add(configID);
                }
                jsonParams.config_ids = jarray;
            }

            return jsonParams;
        }
        #endregion Public Methods
    }
}
