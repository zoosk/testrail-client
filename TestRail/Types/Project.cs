using Newtonsoft.Json.Linq;
using System;

namespace TestRail.Types
{
    /// <summary>Stores information about a project.</summary>
    public class Project : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the project.</summary>
        public ulong Id { get; private set; }

        /// <summary>Name of the project.</summary>
        public string Name { get; set; }

        /// <summary>URL of the project.</summary>
        public string Url { get; private set; }

        /// <summary>Announcement associated with the project.</summary>
        public string Announcement { get; set; }

        /// <summary>True if the announcement should be displayed on the project's overview page and false otherwise.</summary>
        public bool? ShowAnnouncement { get; set; }

        /// <summary>True if the project has been completed.</summary>
        public bool? IsCompleted { get; set; }

        /// <summary>Date on which the milestone was completed.</summary>
        public DateTime? CompletedOn { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses JSON into a project.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Project corresponding to the JSON.</returns>
        public static Project Parse(JObject json)
        {
            var project = new Project
            {
                JsonFromResponse = json,
                Id = (ulong)json["id"],
                Name = (string)json["name"],
                Announcement = (string)json["announcement"],
                ShowAnnouncement = (bool?)json["show_announcement"],
                IsCompleted = (bool?)json["is_completed"],
                Url = (string)json["url"],
                CompletedOn = null == (int?)json["completed_on"] ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["completed_on"])
            };

            return project;
        }

        /// <summary>Creates a json object for this class.</summary>
        /// <returns>JSON object that represents this class.</returns>
        public JObject GetJson()
        {
            dynamic jsonParams = new JObject();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                jsonParams.name = Name;
            }

            if (!string.IsNullOrWhiteSpace(Announcement))
            {
                jsonParams.announcement = Announcement;
            }

            if (null != ShowAnnouncement)
            {
                jsonParams.show_announcement = ShowAnnouncement.Value;
            }

            if (null != IsCompleted)
            {
                jsonParams.is_completed = IsCompleted.Value;
            }

            return jsonParams;
        }
        #endregion Public Methods
    }
}
