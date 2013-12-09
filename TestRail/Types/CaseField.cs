using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TestRail.Types
{
    /// <summary>stores information about a case field</summary>
    public class CaseField
    {
        #region Public Properties
        /// <summary>id of the field</summary>
        public ulong? ID { get; private set; }

        /// <summary>easy name of the custom case field</summary>
        public string Name { get; private set; }

        /// <summary>system name of the custom case field</summary>
        public string SystemName { get; private set; }

        /// <summary>entity id</summary>
        public ulong? EntityID { get; private set; }

        /// <summary>display label for the custom case field</summary>
        public string Label { get; private set; }

        /// <summary>description of the custom case field</summary>
        public string Description { get; private set; }

        /// <summary>type of custom case field as described by the case type</summary>
        public ulong? TypeID { get; private set; }

        /// <summary>location id</summary>
        public ulong? LocationID { get; private set; }

        /// <summary>display order</summary>
        public ulong? DisplayOrder { get; private set; }

        /// <summary>list of configurations for this case field</summary>
        public List<Config> Configs { get; private set; }

        /// <summary>is multi</summary>
        public bool? IsMulti { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>string representation of the object</summary>
        /// <returns>string representation of the object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses the json object into a CaseField object</summary>
        /// <param name="json">json to parse into a CaseField</param>
        /// <returns>CaseField corresponding to the json</returns>
        public static CaseField Parse(JObject json)
        {
            CaseField cf = new CaseField();
            cf.ID = (ulong?)json["id"];
            cf.Name = (string)json["name"];
            cf.SystemName = (string)json["system_name"];
            cf.EntityID = (ulong?)json["entity_id"];
            cf.Label = (string)json["label"];
            cf.Description = (string)json["description"];
            cf.TypeID = (ulong?)json["type_id"];
            cf.LocationID = (ulong?)json["location_id"];
            cf.DisplayOrder = (ulong?)json["display_order"];
            JArray jarray = json["configs"] as JArray;
            if (null != jarray)
            {
                cf.Configs = JsonUtility.ConvertJArrayToList<Config>(jarray, Config.Parse);
            }
            cf.IsMulti = (bool?)json["is_multi"];
            return cf;
        }

        #endregion Public Methods
    }
}
