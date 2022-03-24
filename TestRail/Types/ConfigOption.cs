using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>Stores information about an option for a case field's configuration.</summary>
    public class ConfigOption : BaseTestRailType
    {
        #region Public Properties
        /// <summary>Is this option required?</summary>
        public bool? IsRequired { get; private set; }

        /// <summary>Default value for the option.</summary>
        public string DefaultValue { get; private set; }

        /// <summary>Format of the option.</summary>
        public string Format { get; private set; }

        /// <summary>Row.</summary>
        public string Rows { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>Parse a JSON object into a Config Option.</summary>
        /// <param name="json">Converts the JSON object to a ConfigOption.</param>
        public static ConfigOption Parse(JObject json)
        {
            var configOption = new ConfigOption
            {
                JsonFromResponse = json,
                IsRequired = (bool?)json["is_required"],
                DefaultValue = (string)json["default_value"],
                Format = (string)json["format"],
                Rows = (string)json["rows"]
            };

            return configOption;
        }
        #endregion Public Methods
    }
}
