using Newtonsoft.Json.Linq;
using System;

namespace TestRail.Types
{
    /// <summary>stores information about a case</summary>
    public class Case
    {
        #region Public Properties
        /// <summary>id of the case</summary>
        public ulong? ID { get; set; }

        /// <summary>title of the case</summary>
        public string Title { get; set; }

        /// <summary>section id of the case</summary>
        public ulong? SectionID { get; set; }

        /// <summary>type id of the case</summary>
        public ulong? TypeID { get; set; }

        /// <summary>priority id of the case</summary>
        public ulong? PriorityID { get; set; }

        /// <summary>references for the case</summary>
        public string References { get; set; }

        /// <summary>the milestone this case was associated with</summary>
        public ulong? MilestoneID { get; set; }

        /// <summary>the user who created this case</summary>
        public ulong? CreatedBy { get; set; }

        /// <summary>creation date</summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>estimate time this case will take</summary>
        public string Estimate { get; set; }

        /// <summary>estimate forecast</summary>
        public string EstimateForecast { get; set; }

        /// <summary>suite id for this case</summary>
        public ulong? SuiteID { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>string representation of the object</summary>
        /// <returns>string representation of the object</returns>
        public override string ToString()
        {
            return Title;
        }

        /// <summary>parses json into a case</summary>
        /// <param name="json">json to parse</param>
        /// <returns>case corresponding to the json</returns>
        public static Case Parse(JObject json)
        {
            Case c = new Case();
            c.ID = (ulong?)json["id"];
            c.Title = (string)json["title"];
            c.SectionID = (ulong?)json["section_id"];
            c.TypeID = (ulong?)json["type_id"];
            c.PriorityID = (ulong?)json["priority_id"];
            c.References = (string)json["refs"];
            c.MilestoneID = (ulong?)json["milestone_id"];
            c.CreatedBy = (ulong)json["created_by"];
            c.CreatedOn = (null == (int?)json["created_on"]) ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["created_on"]);
            c.Estimate = (string)json["estimate"];
            c.EstimateForecast = (string)json["estimate_forecast"];
            c.SuiteID = (ulong)json["suite_id"];

            return c;
        }

        /// <summary>creates a json object with the given parameters</summary>
        /// <param name="title">title of the case</param>
        /// <param name="typeID">(optional)the ID of the case type</param>
        /// <param name="priorityID">(optional)the id of the case priority</param>
        /// <param name="estimate">(optional)the estimate, e.g. "30s" or "1m 45s"</param>
        /// <param name="milestoneID">(optional)the ID of the milestone to link to the test case</param>
        /// <param name="refs">(optional)a comma-separated list of references/requirements</param>
        /// <returns>json object for case</returns>
        public virtual JObject GetJson()
        {
            dynamic jsonParams = new JObject();
            if (!string.IsNullOrWhiteSpace(Title)) { jsonParams.title = Title; }
            if (null != TypeID) { jsonParams.type_id = TypeID.Value; }
            if (null != PriorityID) { jsonParams.priority_id = PriorityID.Value; }
            if (!string.IsNullOrWhiteSpace(Estimate)) { jsonParams.estimate = Estimate; }
            if (null != MilestoneID) { jsonParams.milestone_id = MilestoneID.Value; }
            if (!string.IsNullOrWhiteSpace(References)) { jsonParams.refs = References; }
            return jsonParams;
        }
        #endregion Public Methods
    }
}
