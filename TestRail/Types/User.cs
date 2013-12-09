using Newtonsoft.Json.Linq;

namespace TestRail.Types
{
    /// <summary>stores information about a user</summary>
    public class User
    {
        #region Public Properties
        /// <summary>
        /// id of the user 
        /// </summary>
        public ulong ID { get; private set; }

        /// <summary>
        /// name of the user
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// email of the user
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// is the user an admin
        /// </summary>
        public bool IsAdmin { get; private set; }

        /// <summary>
        /// role id of the user
        /// </summary>
        public ulong? RoleID { get; private set; }

        /// <summary>
        /// Is the user active 
        /// </summary>
        public bool IsActive { get; private set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>
        /// Displays the User's ID : User Name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", ID, Name);
        }

        /// <summary>
        /// Parses the json object and returns an User object
        /// </summary>
        /// <param name="json">json to parse</param>
        /// <returns>a user object corresponding to the json object</returns>
        public static User Parse(JObject json)
        {
            User u = new User();
            u.ID = (ulong)json["id"];
            u.Name = (string)json["name"];
            u.Email = (string)json["email"];
            u.IsAdmin = json.Value<bool?>("is_admin") ?? false;
            u.RoleID = (ulong?)json["role_id"];
            u.IsActive = (bool)json["is_active"];
            return u;
        }
        #endregion Public Methods
    }
}
