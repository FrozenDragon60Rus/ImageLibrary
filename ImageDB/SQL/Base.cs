using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace ImageDB.SQL
{
    [SupportedOSPlatform("Windows")]
    public abstract class Base
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

        protected void Send(SqlCommand command)
        {
            try
            {
                Connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { Connection.Close(); }
        }
        public void Clear()
        {
            string commandText = $"DBCC CHECKIDENT(Image, RESEED, 0) " +
                                 $@"DELETE FROM {Table}";
            SqlCommand command = new(commandText, Connection);
            Send(command);
        }
		public void Clear(string[] join)
		{
			Clear();
			string commandText;
			SqlCommand command;
			foreach (var item in join)
			{
				commandText = $"DELETE FROM {Table}BY{item}\r\n;";
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
    }
}
