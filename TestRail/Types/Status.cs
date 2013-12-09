using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores information about a status</summary>
    public class Status
    {
        #region Public Properties
        /// <summary>id of the status</summary>
        public ulong ID { get; private set; }

        /// <summary></summary>
        public string Name { get; private set; }

        /// <summary></summary>
        public string label { get; private set; }

        /// <summary></summary>
        public ulong ColorDark { get; private set; }

        /// <summary></summary>
        public ulong ColorMedium { get; private set; }

        /// <summary></summary>
        public ulong ColorBright { get; private set; }

        /// <summary></summary>
        public bool IsSystem { get; private set; }

        /// <summary></summary>
        public bool IsUntested { get; private set; }

        /// <summary></summary>
        public bool IsFinal { get; private set; }

        #endregion Public Properties

        #region Public Methods
        /// <summary>string representation of the object</summary>
        /// <returns>string representation of the object</returns>	
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses json into a Status object</summary>
        /// <param name="json">json to parse</param>
        /// <returns>Status object corresponding to the json</returns>
        public static Status Parse(JObject json)
        {
            Status s = new Status();
            s.ID = (ulong)json["id"];
            s.Name = (string)json["name"];
            s.label = (string)json["label"];
            s.ColorDark = (ulong)json["color_dark"];
            s.ColorMedium = (ulong)json["color_medium"];
            s.ColorBright = (ulong)json["color_bright"];
            s.IsSystem = (bool)json["is_system"];
            s.IsUntested = (bool)json["is_untested"];
            s.IsFinal = (bool)json["is_final"];
            return s;
        }
        #endregion Public Methods
    }
}
