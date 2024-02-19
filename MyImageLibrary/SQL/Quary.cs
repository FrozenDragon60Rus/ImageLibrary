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
        public static string Column(string[] keys) =>
            string.Join(",", keys);
        public static string Value(string[] keys)
        {
            string[] value = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
                value[i] = "@" + keys[i];
            return string.Join(",", value);
        }
        public static string ColumnWithValue(string[] keys)
        {
            string[] query = new string[keys.Length];

            for (int i = 0;i < keys.Length;i++)
                query[i] = $"{keys[i]}=@new{keys[i]}";

            return string.Join(",", query);
        }
        public static string ColumnWithValue(string[] keys, int[] blackIndex)
        {
            string[] query = new string[keys.Length - blackIndex.Length];

            int index = 0;
            for (int i = 0; i < keys.Length; i++)
                if (blackIndex.Contains(i))
                    continue;
                else
                    query[index++] = $"{keys[i]}=@new{keys[i]}";

            return string.Join(",", query);
        }
        public static string JoinTable(string Table, string[] join)
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
