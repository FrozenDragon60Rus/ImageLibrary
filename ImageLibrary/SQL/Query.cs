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

			return $"IF OBJECT_ID('tempdb.dbo.#List{marker}', 'U') IS NOT NULL " +
						$"DROP TABLE #List{marker};" +
				   $"CREATE TABLE #List{marker} " +
				   $"({marker} VARCHAR(50));" +
				   $"INSERT INTO #List{marker} " +
				   $"VALUES {string.Join(',', strParameter)};";
		}
		public static string Join(string table, IEnumerable<string> join, Filter filter)
		{
			string commandText = string.Empty;
			foreach (var marker in join)
			{
				string crossTable = $"{table}By{marker}";
				commandText += "LEFT JOIN(" +
								   $"SELECT {crossTable}.{table}_Id, STRING_AGG(CAST({marker}.Name AS VARCHAR(1024)), ',') AS {marker} " +
								   $"FROM {crossTable} " +
								   $"LEFT JOIN {marker} " +
								   $"ON {crossTable}.{marker}_Id = {marker}.Id " +
								   Where(table, marker, filter.Marker[marker]) +
								   $"GROUP BY {crossTable}.{table}_Id " +
							  $") AS Join{marker} " +
							  $"ON {table}.Id = Join{marker}.{table}_Id ";
			}
			return commandText;
		}
		public static string Where(Filter filter)
		{
			List<string> filters = [];

            foreach (var item in filter.Marker.Keys)
				if (filter.Marker[item].Count > 0) 
					filters.Add($"Join{item}.{item} IS NOT NULL ");

            int index = filters.Count;
			while(index-- > 0)
				if (filters[index].Length == 0) filters.Remove(filters[index]);
			return filters.Count > 0 ? "AND " + string.Join(" OR ", filters)
									 : string.Empty;
											
		}
		public static string Where<T>(string table, string marker, IEnumerable<T> parameter)
		{
			ArgumentNullException.ThrowIfNull(parameter);
			if (!parameter.Any()) return string.Empty;

			string crossTable = $"{table}By{marker}";

			return $"WHERE {crossTable}.{table}_Id = ALL(" +
						$"SELECT {table}_Id " +
						$"FROM #List{marker} " +
						 "LEFT JOIN (" +
							$"SELECT {crossTable}.{table}_Id, {marker}.Name " +
							$"FROM {crossTable} " +
							$"LEFT JOIN {marker} " +
							$"ON {crossTable}.{marker}_Id = {marker}.Id " +
						 ") as List_Info " +
						$"ON #List{marker}.{marker} = List_Info.Name) ";
		}
	}
}
