using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>Stores information about a case type.</summary>
    public class CaseType : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the case type.</summary>
        public ulong? Id { get; protected set; }

        /// <summary>Name of the case type.</summary>
        public string Name { get; protected set; }

        /// <summary>Is the case type the default?</summary>
        public bool? IsDefault { get; protected set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses JSON into a suite</summary>
        /// <param name="json">JSON to parse</param>
        /// <returns>Suite corresponding to the JSON.</returns>
        public static CaseType Parse(JObject json)
        {
            var caseType = new CaseType
            {
                JsonFromResponse = json,
                Id = (ulong?)json["id"],
                Name = (string)json["name"],
                IsDefault = (bool?)json["is_default"]
            };

            return caseType;
        }
        #endregion Public Methods
    }
}
