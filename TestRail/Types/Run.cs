using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TestRail.Types
{
    /// <summary>Stores information about a run.</summary>
    public class Run : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the run.</summary>
        public ulong? Id { get; private set; }

        /// <summary>Name of the run.</summary>
        public string Name { get; set; }

        /// <summary>Description of the run.</summary>
        public string Description { get; set; }

        /// <summary>ID of the suite associated with the run.</summary>
        public ulong? SuiteId { get; set; }

        /// <summary>ID of the milestone associated with the run.</summary>
        public ulong? MilestoneId { get; set; }

        /// <summary>Config for the run.</summary>
        public string Config { get; private set; }

        /// <summary>True if the run has been completed.</summary>
        public bool? IsCompleted { get; private set; }

        /// <summary>Date on which the run which was completed.</summary>
        public DateTime? CompletedOn { get; private set; }

        /// <summary>Date on which the run which was created.</summary>
        public DateTime? CreatedOn { get; private set; }

        /// <summary>Number of tests in the plan that passed.</summary>
        public uint? PassedCount { get; private set; }

        /// <summary>Number of tests in the plan that are blocked.</summary>
        public uint? BlockedCount { get; private set; }

        /// <summary>Number of tests in the plan that are untested.</summary>
        public uint? UntestedCount { get; private set; }

        /// <summary>Number of tests in the plan that need to be retested.</summary>
        public uint? RetestCount { get; private set; }

        /// <summary>Number of tests in the plan that failed.</summary>
        public uint? FailedCount { get; private set; }

        /// <summary>ID of the project associated with the run.</summary>
        public ulong? ProjectId { get; private set; }

        /// <summary>ID of the plan associated with the run.</summary>
        public ulong? PlanId { get; private set; }

        /// <summary>Is of the user it is assigned to.</summary>
        public ulong? AssignedTo { get; set; }

        /// <summary>True if the test run includes all test cases and false otherwise.</summary>
        public bool IncludeAll { get; set; }

        /// <summary>The amount of tests in the test run with the respective custom status.</summary>
        public ulong CustomStatus1Count { get; private set; }

        /// <summary>The amount of tests in the test run with the respective custom status.</summary>
        public ulong CustomStatus2Count { get; private set; }

        /// <summary>The amount of tests in the test run with the respective custom status.</summary>
        public ulong CustomStatus3Count { get; private set; }

        /// <summary>The amount of tests in the test run with the respective custom status.</summary>
        public ulong CustomStatus4Count { get; private set; }

        /// <summary>The amount of tests in the test run with the respective custom status.</summary>
        public ulong CustomStatus5Count { get; private set; }

        /// <summary>The amount of tests in the test run with the respective custom status.</summary>
        public ulong CustomStatus6Count { get; private set; }

        /// <summary>The amount of tests in the test run with the respective custom status.</summary>
        public ulong CustomStatus7Count { get; private set; }

        /// <summary>The address/URL of the test run in the user interface.</summary>
        public string Url { get; private set; }

        /// <summary>An array of case IDs for the custom case selection.</summary>
        public HashSet<ulong> CaseIds { get; set; }

        /// <summary>An array of case IDs for the custom case selection.</summary>
        public List<ulong> ConfigIds { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses JSON into a run.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Run corresponding to the JSON.</returns>
        public static Run Parse(JObject json)
        {
            var run = new Run
            {
                JsonFromResponse = json,
                Id = (ulong?)json["id"],
                Name = (string)json["name"],
                Description = (string)json["description"],
                SuiteId = (ulong?)json["suite_id"],
                MilestoneId = (ulong?)json["milestone_id"],
                Config = (string)json["config"],
                IsCompleted = (bool?)json["is_completed"],
                CompletedOn = null == (int?)json["completed_on"] ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["completed_on"]),
                CreatedOn = null == (int?)json["created_on"] ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["created_on"]),
                PassedCount = (uint?)json["passed_count"],
                BlockedCount = (uint?)json["blocked_count"],
                UntestedCount = (uint?)json["untested_count"],
                RetestCount = (uint?)json["retest_count"],
                FailedCount = (uint?)json["failed_count"],
                ProjectId = (ulong?)json["project_id"],
                PlanId = (ulong?)json["plan_id"],
                CustomStatus1Count = (ulong)json["custom_status1_count"],
                CustomStatus2Count = (ulong)json["custom_status2_count"],
                CustomStatus3Count = (ulong)json["custom_status3_count"],
                CustomStatus4Count = (ulong)json["custom_status4_count"],
                CustomStatus5Count = (ulong)json["custom_status5_count"],
                CustomStatus6Count = (ulong)json["custom_status6_count"],
                CustomStatus7Count = (ulong)json["custom_status7_count"],
                AssignedTo = (ulong?)json["assignedto_id"],
                IncludeAll = (bool)json["include_all"],
                Url = (string)json["url"]
            };

            return run;
        }

        /// <summary>Creates a JSON object for this class.</summary>
        /// <returns>JSON object that represents this class.</returns>
        public JObject GetJson()
        {
            dynamic jsonParams = new JObject();

            if (null != SuiteId)
            {
                jsonParams.suite_id = SuiteId;
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                jsonParams.name = Name;
            }

            if (null != Description)
            {
                jsonParams.description = Description;
            }

            if (null != MilestoneId)
            {
                jsonParams.milestone_id = MilestoneId;
            }

            if (null != AssignedTo)
            {
                jsonParams.assignedto_id = AssignedTo;
            }

            jsonParams.include_all = IncludeAll;

            if (null != CaseIds && 0 < CaseIds.Count)
            {
                var jarray = new JArray();

                foreach (var caseId in CaseIds)
                {
                    jarray.Add(caseId);
                }

                jsonParams.case_ids = jarray;
            }

            if (null != ConfigIds && 0 < ConfigIds.Count)
            {
                var jarray = new JArray();

                foreach (var configId in ConfigIds)
                {
                    jarray.Add(configId);
                }

                jsonParams.config_ids = jarray;
            }

            return jsonParams;
        }
        #endregion Public Methods
    }
}
