using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>Stores information about a user.</summary>
    public class User : BaseTestRailType
    {
        #region Public Properties
        /// <summary>ID of the user.</summary>
        public ulong Id { get; private set; }

        /// <summary>Name of the user.</summary>
        public string Name { get; private set; }

        /// <summary>Email of the user.</summary>
        public string Email { get; private set; }

        /// <summary>Is the user an admin?</summary>
        public bool IsAdmin { get; private set; }

        /// <summary>Role id of the user.</summary>
        public ulong? RoleId { get; private set; }

        /// <summary>Is the user active?</summary>
        public bool IsActive { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>Displays the User's information</summary>
        /// <returns>A string representation of the User object in the format 'ID : User Name'</returns>
        public override string ToString()
        {
            return $"{Id}:{Name}";
        }

        /// <summary>Parses the JSON object and returns an User object.</summary>
        /// <param name="json">JSON to parse</param>
        /// <returns>A user object corresponding to the JSON object.</returns>
        public static User Parse(JObject json)
        {
            var user = new User
            {
                JsonFromResponse = json,
                Id = (ulong)json["id"],
                Name = (string)json["name"],
                Email = (string)json["email"],
                IsAdmin = json.Value<bool?>("is_admin") ?? false,
                RoleId = (ulong?)json["role_id"],
                IsActive = (bool)json["is_active"]
            };

            return user;
        }
        #endregion Public Methods
    }
}
