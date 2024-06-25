using ImageDB.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace ImageDB.SQL
{
    public abstract class Base : IDisposable
    {
        protected readonly string Name,
                                  Server = @"FROZENDRAGON\SQLSERV",
                                  Table;
        protected string[] columns;
        protected SqlConnection Connection { get; }
        public ConnectionState State { get => Connection.State; }
        public string[] Columns { get => columns; }

        public Base(string name, string table)
        {
            Name = name;
            Table = table;
            Connection = Connect();
            columns = ColumnName();
        }

        protected SqlConnection Connect() =>
            new($@"Server={Server}; " +
                  "Integrated security=SSPI; " +
                  "User=admin; " +
                  "Password=admin; " +
                $@"DataBase={Name}; " +
                  "Trusted_Connection=False; " +
                  "TrustServerCertificate=True;");

        public abstract IEnumerable<T> Get<T>() where T : IData, new(); 

        protected void Send(SqlCommand command)
        {
            try
            {
                Connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            finally { Connection.Close(); }
        }
        public virtual void Clear()
        {
            string commandText = $"DBCC CHECKIDENT({Table}, RESEED, 0) " +
                                 $@"DELETE FROM {Table}";
            SqlCommand command = new(commandText, Connection);
            Send(command);
        }
		public virtual void Clear(string[] join)
		{
			Clear();
			string commandText;
			SqlCommand command;
			foreach (var item in join)
			{
				commandText = $"DELETE FROM ImageBY{item}\r\n;";
				command = new(commandText, Connection);
				Send(command);
			}
		}
		protected string[] ColumnName()
        {
            List<string> column = [];
            string commandText = $@"SELECT name FROM sys.dm_exec_describe_first_result_set('SELECT * FROM {Table}', NULL, 0) ;";
            SqlCommand command = new(commandText, Connection);
            Connection.Open();

            using (SqlDataReader dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    column.Add(dataReader["name"].ToString());

            Connection.Close();
            return [..column];
        }

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
				Debug.WriteLine(e.Message);
			}
			finally { Connection.Close(); }
			return table;
		}
		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
