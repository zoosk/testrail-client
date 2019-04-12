using EnumStringValues;

namespace TestRail.Enums
{
    /// <summary>Command types available</summary>
    public enum CommandType
    {
        /// <summary>For get commands.</summary>
        [StringValue("get")]
        Get,

        /// <summary>For add commands.</summary>
        [StringValue("add")]
        Add,

        /// <summary>For update commands.</summary>
        [StringValue("update")]
        Update,

        /// <summary>For delete commands.</summary>
        [StringValue("delete")]
        Delete,

        /// <summary>For close commands.</summary>
        [StringValue("close")]
        Close
    }
}
