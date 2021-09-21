using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace TestRail.Types
{
    /// <inheritdoc />
    /// <summary>stores information about a case</summary>
    public class Case : BaseTestRailType
    {
        #region Public Properties
        /// <summary>id of the case</summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public ulong? Id { get; set; }

        /// <summary>title of the case</summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        /// <summary>section id of the case</summary>
        [JsonProperty(PropertyName = "section_id")]
        public ulong? SectionId { get; set; }

        /// <summary>type id of the case</summary>
        [JsonProperty(PropertyName = "type_id")]
        public ulong? TypeId { get; set; }

        /// <summary>priority id of the case</summary>
        [JsonProperty(PropertyName = "priority_id")]
        public ulong? PriorityId { get; set; }

        /// <summary>references for the case</summary>
        [JsonProperty(PropertyName = "refs")]
        public string References { get; set; }

        /// <summary>the milestone this case was associated with</summary>
        [JsonProperty(PropertyName = "milestone_id")]
        public ulong? MilestoneId { get; set; }

        /// <summary>the user who created this case</summary>
        [JsonProperty(PropertyName = "created_by")]
        public ulong? CreatedBy { get; set; }

        /// <summary>creation date</summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>estimate time this case will take</summary>
        [JsonProperty(PropertyName = "estimate")]
        public string Estimate { get; set; }

        /// <summary>estimate forecast</summary>
        [JsonProperty(PropertyName = "estimate_forecast")]
        public string EstimateForecast { get; set; }

        /// <summary>suite id for this case</summary>
        [JsonProperty(PropertyName = "suite_id")]
        public ulong? SuiteId { get; set; }

        /// <summary>id of the template (field layout)</summary>
        public ulong? TemplateId { get; set; }
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
            var newCase = new Case
            {
                JsonFromResponse = json,
                Id = (ulong?)json["id"],
                Title = (string)json["title"],
                SectionId = (ulong?)json["section_id"],
                TypeId = (ulong?)json["type_id"],
                PriorityId = (ulong?)json["priority_id"],
                References = (string)json["refs"],
                MilestoneId = (ulong?)json["milestone_id"],
                CreatedBy = (ulong)json["created_by"],
                CreatedOn = null == (int?)json["created_on"] ? null : new DateTime(1970, 1, 1).AddSeconds((int)json["created_on"]),
                Estimate = (string)json["estimate"],
                EstimateForecast = (string)json["estimate_forecast"],
                SuiteId = (ulong)json["suite_id"]
            };

            return newCase;
        }

        /// <summary>creates a json object with the given parameters</summary>
        /// <returns>json object for case</returns>
        public virtual JObject GetJson()
        {
            dynamic jsonParams = new JObject();

            if (!string.IsNullOrWhiteSpace(Title)) { jsonParams.title = Title; }
            if (null != TypeId) { jsonParams.type_id = TypeId.Value; }
            if (null != PriorityId) { jsonParams.priority_id = PriorityId.Value; }
            if (!string.IsNullOrWhiteSpace(Estimate)) { jsonParams.estimate = Estimate; }
            if (null != MilestoneId) { jsonParams.milestone_id = MilestoneId.Value; }
            if (!string.IsNullOrWhiteSpace(References)) { jsonParams.refs = References; }

            return jsonParams;
        }
        #endregion Public Methods
    }
}
