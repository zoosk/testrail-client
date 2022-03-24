using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>Stores information about a status.</summary>
    public class Status : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the status.</summary>
        public ulong Id { get; private set; }

        /// <summary>Name of the status.</summary>
        public string Name { get; private set; }

        /// <summary>Display name of the current status.</summary>
        public string Label { get; private set; }

        /// <summary>The 'dark' color being used for the current status.</summary>
        public ulong ColorDark { get; private set; }

        /// <summary>The 'medium' color being used for the current status.</summary>
        public ulong ColorMedium { get; private set; }

        /// <summary>The 'bright' color being used for the current status.</summary>
        public ulong ColorBright { get; private set; }

        /// <summary>Whether or not the current instance is a system status.</summary>
        public bool IsSystem { get; private set; }

        /// <summary>Whether or not the current instance is untested.</summary>
        public bool IsUntested { get; private set; }

        /// <summary>Whether or not the current instance is finalized.</summary>
        public bool IsFinal { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />	
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses JSON into a Status object.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Status object corresponding to the JSON.</returns>
        public static Status Parse(JObject json)
        {
            var status = new Status
            {
                JsonFromResponse = json,
                Id = (ulong)json["id"],
                Name = (string)json["name"],
                Label = (string)json["label"],
                ColorDark = (ulong)json["color_dark"],
                ColorMedium = (ulong)json["color_medium"],
                ColorBright = (ulong)json["color_bright"],
                IsSystem = (bool)json["is_system"],
                IsUntested = (bool)json["is_untested"],
                IsFinal = (bool)json["is_final"]
            };

            return status;
        }
        #endregion Public Methods
    }
}
