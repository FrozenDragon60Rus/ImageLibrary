using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;
using ImageDB.Table;
using System.Data;
using System.Runtime.Versioning;
using System.Diagnostics;

namespace ImageDB.SQL
{
    [SupportedOSPlatform("Windows")]
    public class DataBase : Base
    {
        string Primary { get; }
        string[] Unique { get; }

        public DataBase(string name, string table) : base(name, table)
        {
            Primary = GetByKeyName("PRIMARY KEY").First();
            Unique = GetByKeyName("UNIQUE");
        }

        public string[] GetByKeyName(string keyName)
        {
            List<string> key = [];
            try
            {
                Connection.Open();
                string coomandText = "SELECT Col.Column_Name from \r\n" +
                                        "INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, \r\n" +
                                        "INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col \r\n" +
                                     "WHERE \r\n" +
                                        "Col.Constraint_Name = Tab.Constraint_Name \r\n" +
                                        "AND Col.Table_Name = Tab.Table_Name \r\n" +
                                       $"AND Tab.Constraint_Type = '{keyName}' \r\n" +
                                       $"AND Col.Table_Name = '{Table}'\r\n";

                SqlCommand command = new(coomandText, Connection);
                using var dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                        key.Add(dataReader[0].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally { Connection.Close(); }

            return key.Count == 0 ? [ string.Empty ]
                                  : [.. key];
        }

        #region Add
        public void Add<T>(T table) where T : IData, new()
        {
            string commandText = $"INSERT INTO {Table} ";

            var keys = columns.Where(k => k != Primary);
            string column = Quary.Column(keys),
                   value = Quary.Value(keys);

            commandText += $"({column}) " +
                           $"VALUES ({value})";
            
            SqlCommand command = new(commandText, Connection);

            foreach (var key in columns)
                command.Parameters.AddWithValue(key, table.Parameter[key]);
            
            Send(command);
            
        }
        
        public void Add<T>(IEnumerable<T> table) where T : IData, new()
        {
            if (!table.Any()) return;
            Clear();

            string commandTextHeader = $"INSERT INTO {Table} ",
                       commandText,
                       column,
                       value;

            var keys = columns.Where(k => k != Primary);
            column = Quary.Column(keys);
            value = Quary.Value(keys);

            commandTextHeader += $"({column}) VALUES ";


            foreach (T data in table)
            {
                commandText = $"({value}) ";
                SqlCommand command = new(commandTextHeader + commandText, Connection);

                foreach (string key in keys)
                    command.Parameters.AddWithValue("@" + key, data.Parameter[key]);
                
                Send(command);
            }
        }
        public void Add(int imageId, int markerId, string marker)
        {
            string commandText = $@"EXEC [dbo].Add{marker} {imageId}, {markerId}";

            SqlCommand command = new(commandText, Connection);
            Send(command);
        }
		#endregion

		#region Load
		public IEnumerable<T> Load<T>() where T : IData, new()
		{
			string commandText = "SELECT * " +
								$"FROM {Table} ";

			return Read<T>(commandText, columns);
		}
		public IEnumerable<T> Load<T>(string[] join) where T : IData, new()
		{
            string commandText =   "SELECT * FROM [dbo].GetImageList();";

			var allColumns = columns.Concat(join);
			return Read<T>(commandText, allColumns);
		}
        public Dictionary<string, object> LoadById(int Id, string[] join)
        {
			string commandText = $@"SELECT * FROM [dbo].GetImageList() WHERE Id={Id};";

            var allColumns = columns.Concat(join);

            return Read<Data>(commandText, allColumns).First().Parameter;
        }
		#endregion

		#region Update
        public void Update<T>(T data) where T : Data
        {
            string commandText = $@"UPDATE {Table} SET Rating = {data.Parameter["Rating"]} WHERE Id = {data.Parameter["Id"]}";
            Send(new(commandText, Connection));
            Debug.WriteLine(commandText);
        }
		#endregion
		public IEnumerable<T> Read<T>(string commandText, IEnumerable<string> columns) where T : IData, new()
		{
			IEnumerable<T> table = [];

			try
			{
				T data;
				SqlCommand command = new(commandText, Connection);
				Connection.Open();
				using var dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					data = new T();

					foreach (var key in columns)
						data.Parameter[key] = dataReader[key];

					table = table.Append(data);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
			finally { Connection.Close(); }
            return table;
		}

		#region Delete
		public void Delete(int imageId)
        {
            string commandText = "DELETE \r\n" +
                                $"FROM {Table} \r\n" +
                                $"WHERE Image_Id = {imageId}";

            SqlCommand command = new(commandText, Connection);
            Send(command);
        }
        public void Delete(int imageId, int markerId)
        {
            string commandText = "DELETE \r\n" +
                                $"FROM {Table} \r\n" +
                                $"WHERE Tag_Id = {markerId}\r\n" +
                                $"AND Image_Id = {imageId}";

            SqlCommand command = new(commandText, Connection);
            Send(command);
        }
        
        public void Delete<T>(T data) where T : IData
        {
            string commandText = "DELETE \r\n" +
                                $"FROM {Table} \r\n" +
                                $"WHERE Id = {data.Parameter["Id"]}";

            SqlCommand command = new(commandText, Connection);
            Send(command);
        }
        public void Delete<T>(List<T> data) where T : IData
        {
            foreach (var _data in data)
            {
                string commandText = "DELETE \r\n" +
                                    $"FROM {Table} \r\n" +
                                    $"WHERE Id = {_data.Parameter["Id"]}";

                SqlCommand command = new(commandText, Connection);
                Send(command);
            }
        }
		#endregion
    }
}
