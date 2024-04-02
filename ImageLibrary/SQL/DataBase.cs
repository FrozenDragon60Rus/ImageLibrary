using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

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

        public List<string> Load(int offset, int count)
        {
            List<string> list = [];

            connection.Open();
            string commandText = "SELECT [Address] " +
                                $"FROM {Table} " +
                                 "ORDER BY Address " +
                                $"OFFSET {offset} ROWS FETCH NEXT {count} ROWS ONLY";

            SqlCommand command = new(commandText, connection);
            using (var dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    list.Add(dataReader["Address"].ToString());

            connection.Close();
            return list;
        }
		public List<string> Load(int offset, int count, Filter filter)
		{
			List<string> list = [];

			connection.Open();
            string commandText = string.Empty;

            foreach (var key in filter.Marker.Keys)
				commandText += Query.TemporaryTable(key, filter.Marker[key]);

			commandText += "SELECT [Address] \r\n" +
						 $"FROM {Table} \r\n" +
						  Query.Join(Table, [..filter.Marker.Keys], filter) +
                         $"WHERE JoinTag.Tag IS NOT NULL OR\r\n" +
                         $"JoinCharacter.Character IS NOT NULL OR\r\n" +
                         $"JoinAuthor.Author IS NOT NULL\r\n" +
                          "ORDER BY Rating DESC \r\n" +
						 $"OFFSET {offset} ROWS FETCH NEXT {count} ROWS ONLY";
            Debug.WriteLine(commandText);

			SqlCommand command = new(commandText, connection);
			using (var dataReader = command.ExecuteReader())
				while (dataReader.Read())
					list.Add(dataReader["Address"].ToString());

			connection.Close();
			return list;
		}

		public List<string> Load(string type)
        {
            List<string> list = [];

            connection.Open();
            string commandText = $"SELECT [{type}] " +
                                 $"FROM {Table} " +
                                 $"ORDER BY {type}";

            SqlCommand command = new(commandText, connection);
            using (var dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    list.Add(dataReader[type].ToString());

            connection.Close();
            return list;
        }
        public List<string> Load(Filter parameter)
        {
            List<string> list = [];

            connection.Open();
            string commandText = $"SELECT [Address] " +
                                 $"FROM {Table} " +
                                 $"ORDER BY Address";

            SqlCommand command = new(commandText, connection);
            /*using (SqlDataReader dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    list.Add(dataReader[type].ToString());*/

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
