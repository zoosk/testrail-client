using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores information about a priority</summary>
    public class Priority
    {
        #region Public Properties
        /// <summary>id of the priority</summary>
        public ulong ID { get; private set; }

        /// <summary>name of the priority</summary>
        public string Name { get; private set; }

        /// <summary>a shortened name of the priority</summary>
        public string ShortName { get; private set; }

        /// <summary>true if the priority is default</summary>
        public bool IsDefault { get; private set; }

        /// <summary>Priority level</summary>
        public int PriorityLevel { get; private set; }
        #endregion Properties

        #region Public Methods
        /// <summary>string representation of the object</summary>
        /// <returns>string representation of the object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>parses json into a plan</summary>
        /// <param name="json">json to parse</param>
        /// <returns>plan corresponding to the json</returns>
        public static Priority Parse(JObject json)
        {
            Priority p = new Priority();
            p.ID = (ulong)json["id"];
            p.Name = (string)json["name"];
            p.ShortName = (string)json["short_name"];
            p.IsDefault = (bool)json["is_default"];
            p.PriorityLevel = (int)json["priority"];
            return p;
        }
        #endregion Public Methods
    }
}
