using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ImageLibrary.SQL
{
	internal static class Query
	{
		public static string TemporaryTable<T>(string marker, IEnumerable<T> parameter)
		{
			Debug.WriteLine(marker + " " + parameter.Count());
			ArgumentNullException.ThrowIfNull(parameter);
			if (!parameter.Any()) return string.Empty;
			
			string[] strParameter = new string[parameter.Count()];
			int index = 0;
			foreach (var item in parameter)
				strParameter[index++] = $"('{item}')";

			string commandText = $"IF OBJECT_ID('tempdb.dbo.#List{marker}', 'U') IS NOT NULL\r\n" +
									 $"DROP TABLE #List{marker};\r\n" +
								 $"CREATE TABLE #List{marker}\r\n" +
								 $"({marker} VARCHAR(50));\r\n";
			commandText += $"INSERT INTO #List{marker}\r\n" +
						   $"VALUES {string.Join(',', strParameter)};\r\n\r\n";

			return commandText;
		}
		public static string Join(string Table, IEnumerable<string> join, Filter filter)
		{
			string commandText = string.Empty;
			foreach (var marker in join)
				commandText += "\r\nLEFT JOIN( \r\n" +
								   $"SELECT {Table}By{marker}.{Table}_Id, STRING_AGG(CAST({marker}.Name AS VARCHAR(1024)), ',') AS {marker} \r\n" +
								   $"FROM {Table}By{marker} \r\n" +
								   $"LEFT JOIN {marker} \r\n" +
								   $"ON {Table}By{marker}.{marker}_Id = {marker}.Id \r\n" +
								   Where(Table, marker, filter.Marker[marker]) + 
								   $"GROUP BY {Table}By{marker}.{Table}_Id \r\n" +
							  $") AS Join{marker} \r\n" +
							  $"ON {Table}.Id = Join{marker}.{Table}_Id\r\n";
			return commandText;
		}
		public static string Where(Filter filter)
		{
			List<string> filters = [];

            foreach (var item in filter.Marker.Keys)
				if (filter.Marker[item].Count > 0) 
					filters.Add($"Join{item}.{item} IS NOT NULL");

            int index = filters.Count;
			while(index-- > 0)
				if (filters[index].Length == 0) filters.Remove(filters[index]);
			return filters.Count > 0 ? "WHERE " + string.Join(" OR\r\n", filters)
									 : string.Empty;
											
		}
		public static string Where<T>(string table, string marker, IEnumerable<T> parameter)
		{
			ArgumentNullException.ThrowIfNull(parameter);
			if (!parameter.Any()) return string.Empty;

			return $"WHERE {table}By{marker}.{table}_Id = ALL(\r\n\t\t\t" +
						$"SELECT {table}_Id \r\n\t\t\t" +
						$"FROM #List{marker}\r\n\t\t\t" +
						 "LEFT JOIN (\r\n\t\t\t\t" +
							$"SELECT {table}By{marker}.{table}_Id, {marker}.Name\r\n\t\t\t\t" +
							$"FROM {table}By{marker}\r\n\t\t\t\t" +
							$"LEFT JOIN {marker}\r\n\t\t\t\t" +
							$"ON {table}By{marker}.{marker}_Id = {marker}.Id\r\n\t\t\t" +
						 ") as List_Info\r\n\t\t\t" +
						$"ON #List{marker}.{marker} = List_Info.Name)\r\n";
		}
	}
}
