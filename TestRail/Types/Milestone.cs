using Newtonsoft.Json.Linq;
using System;

namespace TestRail.Types
{
    /// <summary>stores information about a milestone</summary>
    public class Milestone
    {
        #region Properties
        /// <summary>id of the milestone</summary>
        public ulong ID { get; private set; }

        /// <summary>name of the milestone</summary>
        public string Name { get; set; }

        /// <summary>description of the milestone</summary>
        public string Description { get; set; }

        /// <summary>true if the milestone is completed</summary>
        public bool? IsCompleted { get; set; }

        /// <summary>date on which the milestone is due</summary>
        public DateTime? DueOn { get; set; }

        /// <summary>date on which the milestone was completed</summary>
        public DateTime? CompletedOn { get; private set; }

        /// <summary>id of the project with which the milestone is associated</summary>
        public ulong ProjectID { get; private set; }

        /// <summary>the url for to view the milestone</summary>
        public string Url { get; private set; }
        #endregion Properties

        #region Public Methods
        /// <summary>string representation of the object</summary>
        /// <returns>string representation of the object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>parses json into a milestone</summary>
        /// <param name="json">json to parse</param>
        /// <returns>milestone corresponding to the json</returns>
        public static Milestone Parse(JObject json)
        {
            Milestone m = new Milestone();
            m.ID = (ulong)json["id"];
            m.Name = (string)json["name"];
            m.Description = (string)json["description"];
            m.IsCompleted = (bool?)json["is_completed"];
            m.DueOn = ((null == (int?)json["due_on"]) ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["due_on"]));
            m.CompletedOn = ((null == (int?)json["completed_on"]) ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["completed_on"]));
            m.ProjectID = (ulong)json["project_id"];
            m.Url = (string)json["url"];
            return m;
        }

        /// <summary>Creates a json object for this class</summary>
        /// <returns>json object that represents this class</returns>
        public JObject GetJson()
        {
            dynamic jsonParams = new JObject();
            if (!string.IsNullOrWhiteSpace(Name)) { jsonParams.name = Name; }
            if (!string.IsNullOrWhiteSpace(Description)) { jsonParams.description = Description; }
            if (null != DueOn) { jsonParams.dueOn = DueOn.Value.ToUnixTimestamp(); }
            if (null != IsCompleted) { jsonParams.is_completed = IsCompleted; }
            return jsonParams;
        }
        #endregion Public Methods

    }
}
