using ImageDB.Table;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDB.SQL
{
    internal class ManyByMany : Base
    {
        string[] join;
        public ManyByMany(string name, string table, string[] join) : base(name, table)
        {
            this.join = join;
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
                if (dataReader.HasRows)
                    foreach (string key in columns.Concat(join))
                        parameter.Add(key, dataReader[key]);

            connection.Close();
            return parameter;
        }


    }
}
