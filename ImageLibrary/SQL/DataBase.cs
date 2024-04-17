using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System;

namespace ImageLibrary.SQL
{
    internal class DataBase
    {
        readonly string Name,
                        Server = @"FROZENDRAGON\SQLSERV",
                        Table;
        readonly string[] Columns;
        readonly SqlConnection connection;
        public ConnectionState State { get => connection.State; }

        public DataBase(string name, string table)
        {
            Name = name;
            Table = table;
            connection = Connect();
            Columns = ColumnName();
        }

        protected SqlConnection Connect() =>
            new($"Server={Server}; " +
                 "Integrated security=SSPI; " +
                 "User=admin; " +
                 "Password=admin; " +
                $"DataBase={Name}; " +
                 "Trusted_Connection=False; " +
                 "TrustServerCertificate=True; ");

        public IEnumerable<string> Load(int offset, int count)
        {
			IEnumerable<string> list = [];

            connection.Open();
            string commandText = "SELECT [Address] " +
                                $"FROM {Table} " +
                                 "ORDER BY Address " +
                                $"OFFSET {offset} ROWS";// FETCH NEXT {count} ROWS ONLY ";

            int index = 0;
            SqlCommand command = new(commandText, connection);
            using (var dataReader = command.ExecuteReader())
                while (dataReader.Read() && index++ < count)
                    list = list.Append(dataReader["Address"].ToString());

            connection.Close();
            return list;
        }
		public IEnumerable<string> Load(int offset, int count, Filter filter)
		{
			IEnumerable<string> list = [];
            if (count < 0) return list;

			connection.Open();
            string commandText = "EXEC GetImageListWithFilter @Tag, @Character, @Author, @RatingFrom, @RaringTo, @Offset, @Count;";

			SqlCommand command = new(commandText, connection);

            command.Parameters.AddWithValue("@Tag", 
                                            filter.Tag.Count > 0 ? string.Join(',', filter.Tag) 
                                                                 : DBNull.Value);
			command.Parameters.AddWithValue("@Character", 
                                            filter.Character.Count > 0 ? string.Join(',', filter.Character) 
                                                                 : DBNull.Value);
			command.Parameters.AddWithValue("@Author", 
                                            filter.Author.Count > 0 ? string.Join(',', filter.Author) 
                                                                 : DBNull.Value);
            command.Parameters.AddWithValue("@RatingFrom", filter.Rating.First());
			command.Parameters.AddWithValue("@RaringTo", filter.Rating.Last());
			command.Parameters.AddWithValue("@Offset", offset);
			command.Parameters.AddWithValue("@Count", count);

            using (var dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    list = list.Append(dataReader["Address"].ToString());
                
            

			connection.Close();
			return list;
		}

		public IEnumerable<string> Load(string type)
        {
			IEnumerable<string> list = [];

            connection.Open();
            string commandText = $"SELECT [{type}] " +
                                 $"FROM {Table} " +
                                 $"ORDER BY {type} ";

            SqlCommand command = new(commandText, connection);
            using (var dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    list = list.Append(dataReader[type].ToString());

            connection.Close();
            return list;
        }

        public int GetRowCount()
        {
            connection.Open();
            string commandText = "SELECT COUNT([Address]) " +
                                 $"FROM {Table} ";

            int count = 0;
            SqlCommand command = new(commandText, connection);
            using (var dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    count = (int)dataReader[0];

            connection.Close();

            return count;
        }
        private string[] ColumnName()
        {
            List<string> column = [];
            string commandText = $@"SELECT name FROM sys.dm_exec_describe_first_result_set('SELECT * FROM {Table}', NULL, 0) ;";
            SqlCommand command = new(commandText, connection);
            connection.Open();

            using (var dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    column.Add(dataReader["name"].ToString());

            connection.Close();
            return [..column];
        }
	}
}
