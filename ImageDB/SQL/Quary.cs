using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageDB.SQL
{
    internal static class Quary
    {
        public static string Column(IEnumerable<string> keys) =>
            keys.Contains(string.Empty) ? throw new Exception("Can't be empty") 
                                        : string.Join(",", keys);
        public static string Value(IEnumerable<string> keys)
        {
            if (keys.Contains(string.Empty)) 
                throw new Exception("Can't be empty");

            string[] value = new string[keys.Count()];

            int index = 0;
            foreach (var key in keys)
                value[index++] = "@" + key;
            return string.Join(",", value);
        }
    }
}
