using Newtonsoft.Json.Linq;
using System;

namespace TestRail.Types
{
    /// <summary>Stores information about a case.</summary>
    public class Case : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the case.</summary>
        public ulong? Id { get; set; }

        /// <summary>Title of the case.</summary>
        public string Title { get; set; }

        /// <summary>Section ID of the case.</summary>
        public ulong? SectionId { get; set; }

        /// <summary>Type ID of the case.</summary>
        public ulong? TypeId { get; set; }

        /// <summary>Priority ID of the case.</summary>
        public ulong? PriorityId { get; set; }

        /// <summary>References for the case.</summary>
        public string References { get; set; }

        /// <summary>The milestone this case was associated with.</summary>
        public ulong? MilestoneId { get; set; }

        /// <summary>The user who created this case.</summary>
        public ulong? CreatedBy { get; set; }

        /// <summary>Creation date.</summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>Estimated time this case will take.</summary>
        public string Estimate { get; set; }

        /// <summary>Estimate forecast.</summary>
        public string EstimateForecast { get; set; }

        /// <summary>Suite ID for this case.</summary>
        public ulong? SuiteId { get; set; }

        /// <summary>ID of the template (field layout).</summary>
        public ulong? TemplateId { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return Title;
        }

        /// <summary>Parses JSON into a case.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Case corresponding to the JSON.</returns>
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
                CreatedOn = null == (int?)json["created_on"] ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["created_on"]),
                Estimate = (string)json["estimate"],
                EstimateForecast = (string)json["estimate_forecast"],
                SuiteId = (ulong)json["suite_id"],
                TemplateId = (ulong?)json["template_id"]
            };

            return newCase;
        }

        /// <summary>Creates a JSON object with the given parameters.</summary>
        /// <returns>JSON object for case.</returns>
        public virtual JObject GetJson()
        {
            dynamic jsonParams = new JObject();

            if (!string.IsNullOrWhiteSpace(Title)) { jsonParams.title = Title; }
            if (null != TypeId) { jsonParams.type_id = TypeId.Value; }
            if (null != PriorityId) { jsonParams.priority_id = PriorityId.Value; }
            if (!string.IsNullOrWhiteSpace(Estimate)) { jsonParams.estimate = Estimate; }
            if (null != MilestoneId) { jsonParams.milestone_id = MilestoneId.Value; }
            if (!string.IsNullOrWhiteSpace(References)) { jsonParams.refs = References; }
            if (null != TemplateId) { jsonParams.template_id = TemplateId.Value; }

            return jsonParams;
        }
        #endregion Public Methods
    }
}
