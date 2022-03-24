using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TestRail.Utils;

namespace TestRail.Types
{
    /// <summary>Stores information about a milestone.</summary>
    public class Milestone : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the milestone.</summary>
        public ulong Id { get; private set; }

        /// <summary>Name of the milestone.</summary>
        public string Name { get; set; }

        /// <summary>Description of the milestone.</summary>
        public string Description { get; set; }

        /// <summary>ID of the parent milestone of the sub-milestone.</summary>
        public ulong? ParentId { private get; set; }

        /// <summary>List of the sub-milestones.</summary>
        public List<Milestone> Milestones { get; set; }

        /// <summary>True if the milestone is completed.</summary>
        public bool? IsCompleted { get; set; }

        /// <summary>True if the milestone is started.</summary>
        public bool? IsStarted { get; set; }

        /// <summary>Date on which the milestone is due.</summary>
        public DateTime? DueOn { get; set; }

        /// <summary>Date on which the milestone was completed.</summary>
        public DateTime? CompletedOn { get; private set; }

        /// <summary>ID of the project with which the milestone is associated.</summary>
        public ulong ProjectId { get; private set; }

        /// <summary>The url for to view the milestone.</summary>
        public string Url { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses JSON into a milestone.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Milestone corresponding to the JSON.</returns>
        public static Milestone Parse(JObject json)
        {
            var milestone = new Milestone
            {
                JsonFromResponse = json,
                Id = (ulong)json["id"],
                Name = (string)json["name"],
                Description = (string)json["description"],
                IsCompleted = (bool?)json["is_completed"],
                IsStarted = (bool?)json["is_started"],
                DueOn = null == (int?)json["due_on"] ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["due_on"]),
                CompletedOn = null == (int?)json["completed_on"] ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["completed_on"]),
                ProjectId = (ulong)json["project_id"],
                Url = (string)json["url"]
            };

            var jarray = json["milestones"] as JArray;
            if (null != jarray)
            {
                milestone.Milestones = JsonUtility.ConvertJArrayToList(jarray, Parse);
            }

            return milestone;
        }

        /// <summary>Creates a JSON object for this class.</summary>
        /// <returns>JSON object that represents this class.</returns>
        public JObject GetJson()
        {
            dynamic jsonParams = new JObject();

            if (!string.IsNullOrWhiteSpace(Name)) { jsonParams.name = Name; }
            if (!string.IsNullOrWhiteSpace(Description)) { jsonParams.description = Description; }
            if (null != ParentId) { jsonParams.parent_id = ParentId.Value; }
            if (null != DueOn) { jsonParams.dueOn = DueOn.Value.ToUnixTimestamp(); }
            if (null != IsCompleted) { jsonParams.is_completed = IsCompleted; }

            return jsonParams;
        }
        #endregion Public Methods
    }
}
