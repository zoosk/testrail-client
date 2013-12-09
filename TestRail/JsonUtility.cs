using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TestRail
{
    /// <summary>
    /// Helper class for Json Objects
    /// </summary>
    internal static class JsonUtility
    {
        /// <summary>Merge two Json Objects</summary>
        /// <param name="obj1">object 1 to merge</param>
        /// <param name="obj2">object 2 to merge</param>
        /// <returns>a non null Json object (NOTE: may be empty)</returns>
        internal static JObject Merge(JObject obj1, JObject obj2)
        {
            if (null == obj1)
            {
                obj1 = new JObject();
            }

            if (null != obj2)
            {
                JToken token = obj2.First;
                while (null != token)
                {
                    obj1.Add(token);
                    token = token.Next;
                }
            }
            return obj1;
        }

        /// <summary>Converts a JArray into a List of type T</summary>
        /// <param name="jarray">JArray to parse</param>
        /// <returns>returns a list of objects corresponding to the json, empty list if nothing exists</returns>
        internal static List<T> ConvertJArrayToList<T>(JArray jarray, Func<JObject, T> parse)
        {
            List<T> list = new List<T>();
            if (null != jarray && null != parse)
            {
                foreach (JObject json in jarray)
                {
                    list.Add(parse(json));
                }
            }
            return list;
        }
    }
}
