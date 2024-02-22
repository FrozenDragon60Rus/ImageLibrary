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
using System.Diagnostics;

namespace ImageDB.SQL
{
    public class DataBase : Base
    {
        readonly string Primary;
        string[] Unique;
        

        public DataBase(string name, string table) : base(name, table)
        {
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

        #region Add
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
            if (table.Count == 0) return;
            Clear();
            Console.WriteLine("Очищено");

            string commandTextHeader = $"INSERT INTO {Table} ",
                       commandText,
                       column,
                       value;

            string[] keys = columns.Where(k => k != Primary).ToArray();
            column = Quary.Column(keys);
            value = Quary.Value(keys);

            commandTextHeader += $"({column}) VALUES ";
            
            try
            {
                connection.Open();

                foreach (T data in table)
                {
                    commandText = $"({value}) ";
                    Console.WriteLine(commandTextHeader + commandText);
                    SqlCommand command = new SqlCommand(commandTextHeader + commandText, connection);

                    foreach (string key in columns)
                        command.Parameters.AddWithValue("@" + key, data.parameter[key].ToString());
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

            SqlCommand command = new SqlCommand(commandText, connection);
            Send(command);
        }
        #endregion

        #region Load
        public void Load<T>(ref List<T> table) where T : Data, new()
        {
            table.Clear();

            string commandText = "SELECT * " +
                                $"FROM {Table} ";

            SqlCommand command = new SqlCommand(commandText, connection);
            T data;
            connection.Open();
            using (SqlDataReader dataReader = command.ExecuteReader())
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
            table.Clear();

            string commandText = "SELECT ";
            commandText += Quary.Column(columns);

            foreach (string j in join)
                commandText += $", Join{j}.{j}";
            commandText += $"\r\nFROM {Table} \r\n";

            commandText += Quary.JoinTable(Table, join);

            SqlCommand command = new SqlCommand(commandText, connection);
            T data;
            connection.Open();
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
        public Dictionary<string, object> LoadById(int Id, string[] join)
        {
            Dictionary<string, object> parameter = new Dictionary<string, object>();
            string commandText = "SELECT ";
            commandText += Quary.Column(columns);

            foreach (string j in join)
                commandText += $", Join{j}.{j}";
            commandText += $"\r\nFROM {Table} \r\n";

            commandText += Quary.JoinTable(Table, join);
            commandText += $"\r\nWHERE Id = {Id}";

            connection.Open();
            SqlCommand command = new SqlCommand(commandText, connection);
            using (SqlDataReader dataReader = command.ExecuteReader())
                if (dataReader.Read())
                    foreach (string key in columns.Concat(join))
                        parameter.Add(key, dataReader[key]);

            connection.Close();
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

            if (XML.Info.folder == string.Empty)
            {
                MessageBox.Show("База не была сформирована или отсутствуют данные о её формировании");
                return;
            }
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

                    commandText = $"USING (SELECT {columnWithValue}) as new \r\n" +
                                  $"ON {Table}.{name} = new.{name} \r\n" +
                                   "WHEN NOT MATCHED THEN \r\n" +
                                  $"INSERT ({column}) \r\n" +
                                  $"VALUES ({value});";
                    //Console.WriteLine(commandTextHeader + commandText);

                    command = new SqlCommand(commandTextHeader + commandText, connection);

                    foreach (string key in columns)
                    {
                        command.Parameters.AddWithValue("@" + key, data.parameter[key].ToString());
                        command.Parameters.AddWithValue("@new" + key, data.parameter[key].ToString());
                    }
                        
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { connection.Close(); }
        }

        /// <summary>
        /// Формирует список файлов находящихся в папке по адресу folder и записыват их в поле name
        /// </summary>
        /// <param name="table">Таблица базы данных</param>
        /// <param name="folder">Адрес папки с файлами</param>
        /// <param name="name">Имя параметра формирующегося из папки</param>
        /// <param name="parameter">Значения столбцов по умолчанию</param>
        public void FromFolder<T>(ref List<T> table, string folder, string name, Dictionary<string, object> parameter) where T : Data, new()
        {
            string[] file = Directory.GetFiles(folder);
            T data;
            table.Clear();
            foreach (var item in file)
            {
                data = new T();
                data.parameter = new Dictionary<string, object> (parameter);
                data.parameter[name] = item;
                table.Add(data);
            }
            Add(table);
        }
        public void Delete(int imageId, int markerId)
        {
            connection.Open();
            string commandText = "DELETE \r\n" +
                                $"FROM {Table} \r\n" +
                                $"WHERE Tag_Id = {markerId})\r\n" +
                                $"AND Image_Id = {imageId}";

            SqlCommand command = new SqlCommand(commandText, connection);
            connection.Close();
        }
        public void Delete(Data data)
        {
            connection.Open();
            string commandText = "DELETE \r\n" +
                                $"FROM {Table} \r\n" +
                                $"WHERE Id = {data.parameter["Id"]})\r\n";

            SqlCommand command = new SqlCommand(commandText, connection);
            connection.Close();
        }
    }
}
