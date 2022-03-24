using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>Stores information about a priority.</summary>
    public class Priority : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the priority.</summary>
        public ulong Id { get; private set; }

        /// <summary>Name of the priority.</summary>
        public string Name { get; private set; }

        /// <summary>A shortened name of the priority.</summary>
        public string ShortName { get; private set; }

        /// <summary>True if the priority is default.</summary>
        public bool IsDefault { get; private set; }

        /// <summary>Priority level.</summary>
        public int PriorityLevel { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Parses JSON into a plan.</summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Plan corresponding to the JSON.</returns>
        public static Priority Parse(JObject json)
        {
            var priority = new Priority
            {
                JsonFromResponse = json,
                Id = (ulong)json["id"],
                Name = (string)json["name"],
                ShortName = (string)json["short_name"],
                IsDefault = (bool)json["is_default"],
                PriorityLevel = (int)json["priority"]
            };

            return priority;
        }
        #endregion Public Methods
    }
}
