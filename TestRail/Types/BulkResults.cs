using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

namespace TestRail.Types
{
    /// <summary>Stores information about the result of a test.</summary>
    public class BulkResults : BaseTestRailType
    {
        #region Public Properties
        /// <summary>List of results.</summary>
        public IList<Result> Results { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder("[");

            for (var index = 0; index < Results.Count; index++)
            {
                var result = Results[index];

                builder.Append($"{result.Id}:{result.Comment}");

                if (index + 1 < Results.Count)
                {
                    builder.Append(", ");
                }
            }

            builder.Append("]");

            return builder.ToString();
        }

        /// <summary>Returns a JSON object that represents this class.</summary>
        /// <returns>JSON object that represents this class.</returns>
        public virtual JObject GetJson()
        {
            var jResults = new JArray();

            foreach (var result in Results)
            {
                jResults.Add(result.GetJson());
            }

            dynamic jsonParams = new JObject();

            jsonParams.results = jResults;

            return jsonParams;
        }
        #endregion Public Methods
    }
}
