using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static string AssignValueToColumn(IEnumerable<string> keys)
        {
            if (keys.Contains(string.Empty))
                throw new Exception("Can't be empty");

            string[] query = new string[keys.Count()];

			int index = 0;
			foreach (var key in keys)
				query[index++] = $"{key}=@new{key}";

            return string.Join(",", query);
        }
        public static string Join(string Table, string[] join)
        {
            string commandText = string.Empty;
            foreach (string tableName in join)
                commandText += "\r\nLEFT JOIN( \r\n" +
                                   $"SELECT {Table}By{tableName}.{Table}_Id, STRING_AGG(CAST({tableName}.Name AS VARCHAR(1024)), ',') AS {tableName} \r\n" +
                                   $"FROM {Table}By{tableName} \r\n" +
                                   $"LEFT JOIN {tableName} \r\n" +
                                   $"ON {Table}By{tableName}.{tableName}_Id = {tableName}.Id \r\n" +
                                   $"GROUP BY {Table}By{tableName}.{Table}_Id \r\n" +
                              $") AS Join{tableName} \r\n" +
                              $"ON {Table}.Id = Join{tableName}.{Table}_Id\r\n";
            return commandText;
        }
    }
}
