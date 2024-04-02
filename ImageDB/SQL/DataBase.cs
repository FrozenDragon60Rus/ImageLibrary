using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;
using System.IO;
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
        public void Add<T>(T table) where T : Data, new()
        {
            string commandText = $"INSERT INTO {Table} ";

            var keys = columns.Where(k => k != Primary).ToArray();
            string column = Quary.Column(keys),
                   value = Quary.Value(keys);

            commandText += $"({column}) " +
                           $"VALUES {value}";
            
            SqlCommand command = new(commandText, Connection);

            foreach (string key in columns)
                command.Parameters.AddWithValue(key, table.Parameter[key]);
            
            Send(command);
            
        }
        
        public void Add<T>(List<T> table) where T : Data, new()
        {
            if (table.Count == 0) return;
            Clear();

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
                SqlCommand command = new(commandTextHeader + commandText, Connection);

                foreach (string key in keys)
                    command.Parameters.AddWithValue("@" + key, data.Parameter[key]);
                
                Send(command);
            }
        }
        public void Add(int imageId, int markerId, string marker)
        {
            string commandText = $"MERGE INTO ImageBy{marker} \r\n" +
                                 $"USING (SELECT {marker}_Id = {markerId}, Image_Id = {imageId}) as new \r\n" +
                                 $"ON dbo.{Table}.Image_Id = new.Image_Id\r\n" +
                                 $"AND dbo.{Table}.{marker}_Id = new.{marker}_Id\r\n" +
                                  "WHEN MATCHED THEN\r\n" +
                                  "DELETE\r\n" +
                                  "WHEN NOT MATCHED THEN \r\n" +
                                  "INSERT \r\n" +
                                 $"VALUES ({imageId}, {markerId});";

            SqlCommand command = new(commandText, Connection);
            Send(command);
        }
        #endregion

        #region Load
        public void Load<T>(ref List<T> table) where T : Data, new()
        {
            table.Clear();

            string commandText = "SELECT * " +
                                $"FROM {Table} ";

            Console.WriteLine (Table);
            SqlCommand command = new(commandText, Connection);
            T data;
            Connection.Open();
            using (var dataReader = command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    data = new();

                    foreach (string key in columns)
                        data.Parameter[key] = dataReader[key];

                    table.Add(data);
                }

            }
            Connection.Close();
        }
        public void Load<T>(ref List<T> table, string[] join) where T : Data, new()
        {
            table.Clear();

            string commandText = "SELECT ";
            commandText += Quary.Column(columns);

            foreach (string j in join)
                commandText += $", Join{j}.{j}";
            commandText += $"\r\nFROM {Table} \r\n";

            commandText += Quary.Join(Table, join);

            SqlCommand command = new(commandText, Connection);
            T data;
            Connection.Open();
            using (var dataReader = command.ExecuteReader())
                while (dataReader.Read())
                {
                    data = new T();

                    foreach (string key in columns.Concat(join))
                        data.Parameter[key] = dataReader[key];

                    table.Add(data);
                }
            Connection.Close();
        }
        public Dictionary<string, object> LoadById(int Id, string[] join)
        {
            Dictionary<string, object> parameter = [];
            string commandText = "SELECT ";
            commandText += Quary.Column(columns);

            foreach (string j in join)
                commandText += $", Join{j}.{j}";
            commandText += $"\r\nFROM {Table} \r\n";

            commandText += Quary.Join(Table, join);
            commandText += $"\r\nWHERE Id = {Id}";

            Connection.Open();
            SqlCommand command = new(commandText, Connection);
            using (SqlDataReader dataReader = command.ExecuteReader())
                if (dataReader.Read())
                    foreach (string key in columns.Concat(join))
                        parameter.Add(key, dataReader[key]);

            Connection.Close();
            return parameter;
        }
        #endregion

        public void Refresh<T>(ref List<T> table, string name) where T : Data
        {
            string commandTextHeader = $"MERGE INTO {Table} \r\n",
                   commandText,
                   column,
                   value,
                   columnWithValue;

            if (XML.Info.Folder == string.Empty)
            {
                MessageBox.Show("База не была сформирована или отсутствуют данные о её формировании");
                return;
            }
            string[] file = Directory.GetFiles(XML.Info.Folder);
            SqlCommand command;

            string[] keys = columns.Where(k => k != Primary).ToArray();
            column = Quary.Column(keys);
            value = Quary.Value(keys);

			string[] extensionList = ["jpg, png, jpeg, gif, bmp"];
			foreach (T data in table)
            {
                columnWithValue = Quary.AssignValueToColumn(keys);

                commandText = $"USING (SELECT {columnWithValue}) as new \r\n" +
                              $"ON {Table}.{name} = new.{name} \r\n" +
                               "WHEN NOT MATCHED THEN \r\n" +
                              $"INSERT ({column}) \r\n" +
                              $"VALUES ({value});";

                command = new(commandTextHeader + commandText, Connection);

                foreach (string key in columns)
                {
                    command.Parameters.AddWithValue("@" + key, data.Parameter[key]);
                    command.Parameters.AddWithValue("@new" + key, data.Parameter[key]);
                }

                Send(command);
            }
            
        }

        /// <summary>
        /// Формирует список файлов находящихся в папке по адресу folder и записыват их в поле name
        /// </summary>
        /// <param name="table">Таблица базы данных</param>
        /// <param name="folder">Адрес папки с файлами</param>
        /// <param name="name">Имя параметра формирующегося из папки</param>
        /// <param name="parameter">Значения столбцов по умолчанию</param>
        public void FromFolder<T>(ref List<T> table, string folder, string name) where T : Data, new()
        {
            var file = Directory.GetFiles(folder);
            T data;
            table.Clear();
            string[] extensionList = ["jpg, png, jpeg, gif, bmp"];
            foreach (var item in file)
            {
                data = new();
                data.Parameter[name] = item;
                table.Add(data);
            }
            Add(table);
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
        
        public void Delete<T>(T data) where T : Data
        {
            string commandText = "DELETE \r\n" +
                                $"FROM {Table} \r\n" +
                                $"WHERE Id = {data.Parameter["Id"]}";

            SqlCommand command = new(commandText, Connection);
            Send(command);
        }
        public void Delete<T>(List<T> data) where T : Data
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
