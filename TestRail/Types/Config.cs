using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores information about a config</summary>
    public class Config
    {
        #region Public Properties
        /// <summary>Options for this configuration</summary>
        public ConfigOption Option { get; private set; }

        /// <summary>Guid unique identifier as a string</summary>
        public string ID { get; private set; }

        /// <summary>Configuration context</summary>
        public ConfigContext Context { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>Constructor</summary>
        /// <param name="json">json object to parse into a Config</param>
        public static Config Parse(JObject json)
        {
            Config c = new Config();
            c.Option = ConfigOption.Parse((JObject)json["options"]);
            c.Context = ConfigContext.Parse((JObject)json["context"]);
            c.ID = (string)json["id"];
            return c;
        }
        #endregion Public Methods
    }
}
