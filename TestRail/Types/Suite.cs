using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>Stores information about a suite.</summary>
    public class Suite : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the suite.</summary>
        public ulong? Id { get; set; }

        /// <summary>Name of the suite.</summary>
        public string Name { get; set; }

        /// <summary>Description of the suite.</summary>
        public string Description { get; set; }

        /// <summary>ID of the project associated with the suite.</summary>
        public ulong? ProjectId { get; set; }

        /// <summary>URL to view the suite.</summary>
        public string Url { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses JSON into a suite.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Suite corresponding to the JSON.</returns>
        public static Suite Parse(JObject json)
        {
            var suite = new Suite
            {
                JsonFromResponse = json,
                Id = (ulong?)json["id"],
                Name = (string)json["name"],
                Description = (string)json["description"],
                ProjectId = (ulong?)json["project_id"],
                Url = (string)json["url"]
            };

            return suite;
        }

        /// <summary>Creates a JSON object for this class.</summary>
        /// <returns>JSON object that represents this class.</returns>
        public JObject GetJson()
        {
            dynamic jsonParams = new JObject();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                jsonParams.name = Name;
            }

            if (!string.IsNullOrWhiteSpace(Description))
            {
                jsonParams.description = Description;
            }

            return jsonParams;
        }
        #endregion Public Methods
    }
}
