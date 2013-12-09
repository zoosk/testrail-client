using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores information about a case type</summary>
    public class CaseType
    {
        #region Public Properties
        /// <summary>ID of the case type</summary>
        public ulong? ID { get; protected set; }

        /// <summary>Name of the case type</summary>
        public string Name { get; protected set; }

        /// <summary>is the case type the default</summary>
        public bool? IsDefault { get; protected set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>string representation of the object</summary>
        /// <returns>string representation of the object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>parses json into a suite</summary>
        /// <param name="json">json to parse</param>
        /// <returns>suite corresponding to the json</returns>
        public static CaseType Parse(JObject json)
        {
            CaseType ct = new CaseType();
            ct.ID = (ulong?)json["id"];
            ct.Name = (string)json["name"];
            ct.IsDefault = (bool?)json["is_default"];
            return ct;
        }
        #endregion Public Methods
    }
}
