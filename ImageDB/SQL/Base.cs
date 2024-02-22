using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageDB.SQL
{
    public abstract class Base
    {
        protected readonly string Name,
                        Server = @"FROZENDRAGON\SQLSERV",
                        Table,
                        Primary;
        protected string[] columns;
        protected readonly SqlConnection connection;
        public ConnectionState State { get => connection.State; }
        public string[] Columns { get => columns; }

        public Base(string name, string table)
        {
            Name = name;
            Table = table;
            connection = Connect();
            columns = ColumnName();
        }

        protected SqlConnection Connect() =>
            new SqlConnection($"Server={Server}; " +
                               "Integrated security=SSPI; " +
                               "User=admin; " +
                               "Password=admin; " +
                              $"DataBase={Name}; " +
                               "Trusted_Connection=False; " +
                               "TrustServerCertificate=True; ");

        protected void Send(SqlCommand command)
        {
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { connection.Close(); }
        }
        public void Clear()
        {
            string commandText = "DELETE FROM Image";
            SqlCommand command = new SqlCommand(commandText, connection);
            Send(command);
        }
        protected string[] ColumnName()
        {
            List<string> column = new List<string>();
            string commandText = $@"SELECT name FROM sys.dm_exec_describe_first_result_set('SELECT * FROM {Table}', NULL, 0) ;";
            SqlCommand command = new SqlCommand(commandText, connection);
            connection.Open();

            using (SqlDataReader dataReader = command.ExecuteReader())
                while (dataReader.Read())
                    column.Add(dataReader["name"].ToString());

            connection.Close();
            return column.ToArray();
        }
    }
}
