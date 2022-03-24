using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>Stores information about a section.</summary>
    public class Section : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the section.</summary>
        public ulong? Id { get; set; }

        /// <summary>Name of the section.</summary>
        public string Name { get; set; }

        /// <summary>Description of the section.</summary>
        public string Description { get; set; }

        /// <summary>ID of the parent section of the section.</summary>
        public ulong? ParentId { get; set; }

        /// <summary>Depth of the section.</summary>
        public uint? Depth { get; set; }

        /// <summary>Display order of the section.</summary>
        public uint? DisplayOrder { get; set; }

        /// <summary>ID of the suite associated with the section.</summary>
        public ulong? SuiteId { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses JSON into a section.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Section corresponding to the JSON.</returns>
        public static Section Parse(JObject json)
        {
            var section = new Section
            {
                JsonFromResponse = json,
                Id = (ulong?)json["id"],
                Name = (string)json["name"],
                Description = (string)json["description"],
                ParentId = (ulong?)json["parent_id"],
                Depth = (uint?)json["depth"],
                DisplayOrder = (uint?)json["display_order"],
                SuiteId = (ulong?)json["suite_id"]
            };

            return section;
        }

        /// <summary>Creates a JSON object for this class.</summary>
        /// <returns>JSON object that represents this class.</returns>
        public JObject GetJson()
        {
            dynamic jsonParams = new JObject();
            if (null != SuiteId) { jsonParams.suite_id = SuiteId.Value; }
            if (null != ParentId) { jsonParams.parent_id = ParentId.Value; }
            if (!string.IsNullOrWhiteSpace(Name)) { jsonParams.name = Name; }
            if (!string.IsNullOrWhiteSpace(Description)) { jsonParams.description = Description; }
            return jsonParams;
        }
        #endregion Public Methods
    }
}
