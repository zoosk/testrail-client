using System;

namespace TestRail
{
    /// <summary>represents the result of a command </summary>
    public class CommandResult : CommandResult<string>
    {
        /// <summary>constructor</summary>
        /// <param name="wasSuccessful">true if the command was successful</param>
        /// <param name="result">result of the command</param>
        /// <param name="e">exception thrown by the command</param>
        public CommandResult(bool wasSuccessful, string result, Exception e = null) : base(wasSuccessful, result, e) { }
    }

    /// <summary>represents the result of a command</summary>
    /// <typeparam name="T">type of the result</typeparam>
    public class CommandResult<T>
    {
        /// <summary>true if the command was successful</summary>
        public bool WasSuccessful { get; set; }
        /// <summary>result of the command</summary>
        public T Value { get; set; }
        /// <summary>exception thrown by the command</summary>
        public Exception Exception { get; set; }

        /// <summary>parameterless constructor</summary>
        public CommandResult()
        {
            WasSuccessful = false;
            Value = default(T);
            Exception = null;
        }

        /// <summary>constructor</summary>
        /// <param name="wasSuccessful">true if the command was successful</param>
        /// <param name="result">result of the command</param>
        /// <param name="e">exception thrown by the command</param>
        public CommandResult(bool wasSuccessful, T result, Exception e=null)
        {
            WasSuccessful = wasSuccessful;
            Value = result;
            Exception = e;
        }
    }
}
