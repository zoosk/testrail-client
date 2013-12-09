using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores information about a test</summary>
    public class Test
    {
        #region Public Properties
        /// <summary>id of the test</summary>
        public ulong? ID { get; private set; }

        /// <summary>id of the test case</summary>
        public ulong? CaseID { get; set; }

        /// <summary>id of the test run</summary>
        public ulong? RunID { get; private set; }

        /// <summary>test rail status id</summary>
        public ResultStatus? Status { get; private set; }

        /// <summary>id of the user the test is assigned to</summary>
        public ulong? AssignedToID { get; private set; }

        /// <summary>title of the test</summary>
        public string Title;
        #endregion

        #region Public Methods
        /// <summary>string representation of the object</summary>
        /// <returns>string representation of the object</returns>
        public override string ToString()
        {
            return Title;
        }

        /// <summary>parses a test from the supplied json</summary>
        /// <param name="json">json for the test</param>
        /// <returns>test corresponding to the json</returns>
        public static Test Parse(JObject json)
        {
            Test t = new Test();
            t.ID = (ulong?)json["id"];
            t.CaseID = (ulong?)json["case_id"];
            t.RunID = (ulong?)json["run_id"];
            t.Status = (ResultStatus?)((int)json["status_id"]);
            t.AssignedToID = (ulong?)json["assignedto_id"];
            t.Title = (string)json["title"];
            return t;
        }
        #endregion
    }
}
