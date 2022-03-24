using Newtonsoft.Json.Linq;
using TestRail.Enums;

namespace TestRail.Types
{
    /// <summary>Stores information about a step.</summary>
    public class Step : BaseTestRailType
    {
        #region Public Fields
        /// <summary>Description of the step.</summary>
        public string Description;

        /// <summary>Expected result for the step.</summary>
        public string Expected;

        /// <summary>Actual result for the step.</summary>
        public string Actual;

        /// <summary>Result of the step.</summary>
        public ResultStatus? Status;
        #endregion Public Fields

        #region Public Methods
        /// <summary>Parses JSON into a step.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Step corresponding to the JSON.</returns>
        public static Step Parse(JObject json)
        {
            var step = new Step
            {
                JsonFromResponse = json,
                Description = (string)json["content"],
                Expected = (string)json["expected"],
                Actual = (string)json["actual"],
                Status = null == (int?)json["status_id"] ? (ResultStatus?)null : (ResultStatus)(int)json["status_id"]
            };

            return step;
        }

        /// <summary>Get the JSON object that describes this class.</summary>
        /// <returns>JSON object for this class.</returns>
        public JObject GetJsonObject()
        {
            dynamic json = new JObject();

            if (!string.IsNullOrWhiteSpace(Description))
            {
                json.content = Description;
            }

            if (!string.IsNullOrWhiteSpace(Expected))
            {
                json.expected = Expected;
            }

            if (!string.IsNullOrWhiteSpace(Actual))
            {
                json.actual = Actual;
            }

            if (null != Status)
            {
                json.status_id = (int)Status;
            }

            return json;
        }
        #endregion Public Methods
    }
}
