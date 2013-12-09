using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores information about a section</summary>
    public class Section
    {
        #region Public Properties

        /// <summary>id of the section</summary>
        public ulong? ID { get; set; }

        /// <summary>name of the section</summary>
        public string Name { get; set; }

        /// <summary>id of the parent section of the section</summary>
        public ulong? ParentID { get; set; }

        /// <summary>depth of the section</summary>
        public uint? Depth { get; set; }

        /// <summary>display order of the section</summary>
        public uint? DisplayOrder { get; set; }

        /// <summary>id of the suite associated with the section</summary>
        public ulong? SuiteID { get; set; }

        #endregion Public Properties

        #region Public Methods
        /// <summary>string representation of the object</summary>
        /// <returns>string representation of the object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>parses json into a section</summary>
        /// <param name="json">json to parse</param>
        /// <returns>section corresponding to the json</returns>
        public static Section Parse(JObject json)
        {
            Section s = new Section();
            s.ID = (ulong?)json["id"];
            s.Name = (string)json["name"];
            s.ParentID = (ulong?)json["parent_id"];
            s.Depth = (uint?)json["depth"];
            s.DisplayOrder = (uint?)json["display_order"];
            s.SuiteID = (ulong?)json["suite_id"];
            return s;
        }

        /// <summary>Creates a json object for this class</summary>
        /// <returns>json object that represents this class</returns>
        public JObject GetJson()
        {
            dynamic jsonParams = new JObject();
            if (null != SuiteID) { jsonParams.suite_id = SuiteID.Value; }
            if (null != ParentID) { jsonParams.parent_id = ParentID.Value; }
            if (!string.IsNullOrWhiteSpace(Name)) { jsonParams.name = Name; }
            return jsonParams;
        }

        #endregion Public Methods
    }
}
