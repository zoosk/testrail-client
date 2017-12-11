using System;
using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores information about the result of a test</summary>
    public class Result : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the result</summary>
        public ulong ID { get; set; }

        /// <summary>ID of the test</summary>
        public ulong TestID { get; set; }

        /// <summary>ID of the test status</summary>
        public ulong? StatusID { get; set; }

        /// <summary>created by</summary>
        public ulong? CreatedBy { get; set; }

        /// <summary>result created on</summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>the ID of the user the test should be assigned to</summary>
        public ulong? AssignedToID { get; set; }

        /// <summary>the comment /description for the test result</summary>
        public string Comment { get; set; }

        /// <summary>the version or build tested against</summary>
        public string Version { get; set; }

        /// <summary>the time it took to execute the test</summary>
        public TimeSpan? Elapsed { get; set; }

        /// <summary>a comma-separated list of defects to link to the test result</summary>
        public string Defects { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>
        /// string representation of the object
        /// </summary>
        /// <returns>string representation of the object</returns>
        public override string ToString()
        {
            return $"{ID}:{Comment}";
        }

        /// <summary>Parse the JSON into a Result</summary>
        /// <param name="json">json object to parse</param>
        /// <returns>a Result</returns>
        public static Result Parse(JObject json)
        {
            var r = new Result
            {
                JsonFromResponse = json,
                ID = (ulong)json["id"],
                TestID = (ulong)json["test_id"],
                StatusID = (ulong?)json["status_id"],
                CreatedBy = (ulong?)json["created_by"],
                CreatedOn = null == (int?)json["created_on"] ? (DateTime?)null : new DateTime(1970, 1, 1).AddSeconds((int)json["created_on"]),
                AssignedToID = (ulong?)json["assignedto_id"],
                Comment = (string)json["comment"],
                Version = (string)json["version"],
                Defects = (string)json["defects"],
            };

            // separate for easier debugging if necessary
            r.Elapsed = TimeSpanUtility.FromString((string)json["elapsed"]);
            return r;
        }

        /// <summary>Returns a json object that represents this class</summary>
        /// <returns>json object that represents this class</returns>
        public virtual JObject GetJson()
        {
            dynamic jsonParams = new JObject();
            if (null != StatusID) { jsonParams.status_id = (int)StatusID; }
            if (null != Comment) { jsonParams.comment = Comment; }
            if (null != Version) { jsonParams.version = Version; }
            if (null != Elapsed) { jsonParams.elapsed = $"{Elapsed.Value.Days}d {Elapsed.Value.Hours}h {Elapsed.Value.Minutes}m {Elapsed.Value.Seconds}s" ; }
            if (null != Defects) { jsonParams.defects = Defects; }
            if (null != AssignedToID) { jsonParams.assignedto_id = AssignedToID.Value; }
            return jsonParams;
        }
        #endregion Public Methods
    }
}
