using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Windows;
using ImageDB.Table;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks.Dataflow;
using System.Runtime.CompilerServices;

namespace ImageDB.SQL
{
    public class ImageDataBase : Base
	{
        public List<Image> Image { get; protected set; }

        string Primary { get; }
        string[] Unique { get; }
        private readonly string[] join;

        public ImageDataBase(string name, string table, string[] join) : base(name, table)
        {
            Primary = GetKeyColumnName("PRIMARY KEY").First();
            Unique = GetKeyColumnName("UNIQUE");

            this.join = join;
            Image = Get<Image>().ToList();
        }

        public string[] GetKeyColumnName(string keyName)
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
                Debug.WriteLine(ex.Message);
            }
            finally { Connection.Close(); }

            return key.Count == 0 ? [ string.Empty ]
                                  : [.. key];
        }

        #region Add
        public void Add<T>(T table) where T : IData, new()
        {
            Image.Add(table as Image);

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

            Image = (table as IEnumerable<Image>).ToList();

            string commandTextHeader = $"INSERT INTO {Table} ",
                       commandText,
                       column,
                       value;

            var keys = columns.Where(k => k != Primary);
            column = Quary.Column(keys);
            value = Quary.Value(keys);

            commandTextHeader += $"({column}) VALUES ";


            foreach (var data in table)
            {
                commandText = $"({value}) ";
                SqlCommand command = new(commandTextHeader + commandText, Connection);

                foreach (string key in keys)
                    command.Parameters.AddWithValue("@" + key, data.Parameter[key]);
                
                Send(command);
            }
        }
		#endregion

		public override void Clear()
		{
			base.Clear(join);

            Image.Clear();
		}

		#region Get
		public override IEnumerable<T> Get<T>()
		{
            string commandText = "SELECT * FROM [dbo].ImageList;";

			var allColumns = columns.Concat(join);
			return Read<T>(commandText, allColumns);
		}

        public virtual Dictionary<string, object> Get<T>(int Id) where T : IData, new()
		{
			string commandText = $@"SELECT * FROM [dbo].ImageList WHERE Id={Id};";

            var allColumns = columns.Concat(join);

            return Read<T>(commandText, allColumns).First().Parameter;
        }
		#endregion

        public Image Update(Image data)
        {
            int index = Image.IndexOf(data);
            Image[index] = new(Get<Image>(data.Id));
            return Image[index];
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
        
        public void Delete<T>(T data) where T : IData
        {
            string commandText = "DELETE \r\n" +
                                $"FROM {Table} \r\n" +
                                $"WHERE Id = {data.Parameter["Id"]}";

            Image.Remove(data as Image);

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

				Image.Remove(data as Image);

				SqlCommand command = new(commandText, Connection);
                Send(command);
            }
        }
		#endregion
	}
}
