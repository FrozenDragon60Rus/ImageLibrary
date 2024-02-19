using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;
using System.IO;
using ImageDB.Table;
using System.Data.Common;
using System.Windows.Input;
using System.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.ComponentModel.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Controls.Primitives;

namespace ImageDB.SQL
{
    internal class DataBase
    {
        readonly string Name,
                        Server = @"FROZENDRAGON\SQLSERV",
                        Table,
                        Primary;
        string[] columns,
                 Unique;
        public string[] Columns { get => columns; }
        readonly SqlConnection connection;
        public ConnectionState State { get => connection.State; }

        public DataBase(string name, string table)
        {
            Name = name;
            Table = table;
            connection = Connect();
            columns = ColumnName();
            Primary = GetByKeyName("PRIMARY KEY").First();
            Unique = GetByKeyName("UNIQUE");
        }

        public string[] GetByKeyName(string keyName)
        {
            List<string> key = new List<string>();
            try
            {
                connection.Open();
                string coomandText = "SELECT Col.Column_Name from \r\n" +
                                        "INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, \r\n" +
                                        "INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col \r\n" +
                                     "WHERE \r\n" +
                                        "Col.Constraint_Name = Tab.Constraint_Name \r\n" +
                                        "AND Col.Table_Name = Tab.Table_Name \r\n" +
                                       $"AND Tab.Constraint_Type = '{keyName}' \r\n" +
                                       $"AND Col.Table_Name = '{Table}'\r\n";

                SqlCommand command = new SqlCommand(coomandText, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                    while (dataReader.Read())
                        key.Add(dataReader[0].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally { connection.Close(); }
            return key.Count == 0 ? new string[] { string.Empty }
                                  : key.ToArray();
        }

        protected SqlConnection Connect() =>
            new SqlConnection($"Server={Server}; " +
                               "Integrated security=SSPI; " +
                               "User=admin; " +
                               "Password=admin; " +
                              $"DataBase={Name}; " +
                               "Trusted_Connection=False; " +
                               "TrustServerCertificate=True; ");
        public void Add<T>(T table) where T : Data, new()
        {
            string commandText = $"INSERT INTO {Table} ";

            string[] keys = columns.Where(k => k != Primary).ToArray();
            string column = Quary.Column(keys),
                   value = Quary.Value(keys);

            commandText += $"({column}) " +
                           $"VALUES {value}";

            SqlCommand command = new SqlCommand(commandText, connection);

            foreach (string key in columns)
                command.Parameters.AddWithValue(key, table.parameter[key]);
            Send(command);
        }
        public void Add<T>(List<T> table) where T : Data, new()
        {
            foreach (string column in columns)
                Console.WriteLine(column);
            if (table.Count == 0) return;
            try
            {
                connection.Open();
                string commandTextHeader = $"INSERT INTO {Table} ",
                       commandText,
                       column,
                       value;

                string[] keys = columns.Where(k => k != Primary).ToArray();
                column = Quary.Column(keys);
                value = Quary.Value(keys);

                commandTextHeader += $"({column}) VALUES ";

                foreach (T data in table)
                {
                    commandText = $"({value}) ";
                    //Console.WriteLine(commandTextHeader + commandText);
                    SqlCommand command = new SqlCommand(commandTextHeader + commandText, connection);

                    foreach (string key in columns)
                        command.Parameters.AddWithValue("@"+key, data.parameter[key].ToString());
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally { connection.Close(); }
        }
        public void Update<T>(T data) where T : Data
        {
            string command = $"UPDATE {Table} " +
                              "SET ";
            Dictionary<string, object> db;
            db = MyConvert.ArrayToDictionary(data.Key(), data.Value());

            foreach (string key in db.Keys)
                command += $"{key} = {db[key]}, ";

            command = command.Remove(command.Length - 2);

            command += $" WHERE Id = {db["Id"]}";
            Send(new SqlCommand(command, connection));
        }
        public void Load<T>(ref List<T> table) where T : Data, new()
        {
            connection.Open();
            string commandText = "SELECT * " +
                                $"FROM {Table} ";

            SqlCommand command = new SqlCommand(commandText, connection);
            T data;
            using(SqlDataReader dataReader = command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    data = new T();
                    data.parameter = new Dictionary<string, object>();
                    foreach (string key in columns)
                        data.parameter.Add(key, dataReader[key]);
                    table.Add(data);
                }
                
            }
            connection.Close();
        }
        public void Load<T>(ref List<T> table, string[] join) where T : Data, new()
        {
            connection.Open();
            string commandText = "SELECT ";
            commandText += Quary.Column(columns);

            foreach (string j in join)
                commandText += $", Join{j}.{j}";
            commandText += $"\r\nFROM {Table} \r\n";

            commandText += Quary.JoinTable(Table, join);

            SqlCommand command = new SqlCommand(commandText, connection);
            T data;
            using (SqlDataReader dataReader = command.ExecuteReader())
                while (dataReader.Read())
                {
                    data = new T();
                    data.parameter = new Dictionary<string, object>();
                    foreach (string key in columns.Concat(join))
                        data.parameter.Add(key, dataReader[key]);
                    table.Add(data);
                }
            connection.Close();
        }
        public Dictionary<string, object> GetLineById(string[] join, int Id)
        {
            connection.Open();
            string commandText = "SELECT ";
            commandText += Quary.Column(columns);

            foreach (string j in join)
                commandText += $", Join{j}.{j}";
            commandText += $"\r\nFROM {Table} \r\n";

            commandText += Quary.JoinTable(Table, join);

            commandText += $"WHERE Id = {Id}";

            SqlCommand command = new SqlCommand(commandText, connection);
            Dictionary<string,object> value = new Dictionary<string,object>();
            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                while (dataReader.Read())
                    foreach (string key in columns.Concat(join))
                        value.Add(key, dataReader[key]);

                connection.Close();
            }
            return value;
        }

        private void Send(SqlCommand command)
        {
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                Console.WriteLine($"{Table} database successfully recorded");
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
            finally { connection.Close(); }
        }
        public void Refresh<T>(ref List<T> table, string name) where T : Data
        {
            string commandTextHeader = $"MERGE INTO {Table} \r\n",
                   commandText,
                   column,
                   value,
                   columnWithValue,
                   columnWithValueWithoutUnique;
            string[] file = Directory.GetFiles(XML.Info.folder);
            SqlCommand command;

            string[] keys = columns.Where(k => k != Primary).ToArray();
            column = Quary.Column(keys);
            value = Quary.Value(keys);

            try
            {
                connection.Open();

                foreach (T data in table)
                {
                    columnWithValue = Quary.ColumnWithValue(keys);
                    columnWithValueWithoutUnique = Quary.ColumnWithValue(keys, new int[] { Array.IndexOf(keys, name) });

                    commandText = $"USING (SELECT {columnWithValue}) as new \r\n" +
                                  $"ON {Table}.{name} = new.{name} \r\n" +
                                   "WHEN MATCHED THEN \r\n" +
                                  $"UPDATE SET {columnWithValueWithoutUnique} \r\n" +
                                   "WHEN NOT MATCHED THEN \r\n" +
                                  $"INSERT ({column}) \r\n" +
                                  $"VALUES ({value});";
                    //Console.WriteLine(commandTextHeader + commandText);

                    command = new SqlCommand(commandTextHeader + commandText, connection);

                    foreach (string key in columns)
                        command.Parameters.AddWithValue("@" + key, data.parameter[key].ToString());
                    foreach (string key in columns)
                        command.Parameters.AddWithValue("@new" + key, data.parameter[key].ToString());

                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
            finally { connection.Close(); }
        }
        public void Update<T>(List<T> table) where T : Data, new()
        {
            Clear();
            Add(table);
        }

        /// <summary>
        /// Формирует список файлов находящихся в папке по адресу folder и записыват их в поле name
        /// </summary>
        /// <param name="table">Таблица базы данных</param>
        /// <param name="folder">Адрес папки с файлами</param>
        /// <param name="name">Имя параметра формирующегося из папки</param>
        /// <param name="parameter">Значения столбцов по умолчанию</param>
        public void FromFolder<T>(ref List<T> table, string folder, string name, Dictionary<string,object> parameter) where T : Data, new()
        {
            string[] file = Directory.GetFiles(folder);
            T data; 
            foreach (var item in file) 
            {
                data = new T();
                data.parameter = parameter;
                data.parameter[name] = item;
                table.Add(data);
            }
            Add(table);
        }
        public void Clear()
        {
            string commandText = "DELETE FROM Image";
            SqlCommand command = new SqlCommand(commandText, connection);
            Send(command);
        }
        private string[] ColumnName()
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
