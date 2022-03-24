using Newtonsoft.Json.Linq;
using TestRail.Enums;

namespace TestRail.Types
{
    /// <summary>Stores information about a test.</summary>
    public class Test : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the test.</summary>
        public ulong? Id { get; private set; }

        /// <summary>ID of the test case.</summary>
        public ulong? CaseId { get; set; }

        /// <summary>ID of the test run.</summary>
        public ulong? RunId { get; private set; }

        /// <summary>TestRail status ID.</summary>
        public ResultStatus? Status { get; private set; }

        /// <summary>ID of the user the test is assigned to.</summary>
        public ulong? AssignedToId { get; private set; }

        /// <summary>Title of the test.</summary>
        public string Title;
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return Title;
        }

        /// <summary>Parses a test from the supplied JSON.</summary>
        /// <param name="json">JSON for the test.</param>
        /// <returns>Test corresponding to the JSON.</returns>
        public static Test Parse(JObject json)
        {
            var test = new Test
            {
                JsonFromResponse = json,
                Id = (ulong?)json["id"],
                CaseId = (ulong?)json["case_id"],
                RunId = (ulong?)json["run_id"],
                Status = (ResultStatus?)(int)json["status_id"],
                AssignedToId = (ulong?)json["assignedto_id"],
                Title = (string)json["title"]
            };

            return test;
        }
        #endregion Public Methods
    }
}
