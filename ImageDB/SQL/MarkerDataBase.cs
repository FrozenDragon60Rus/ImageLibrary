using ImageDB.Table;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ImageDB.SQL
{
	public class MarkerDataBase(string name, string table) : Base(name, table)
	{
		public override IEnumerable<T> Get<T>()
		{
			string commandText = "SELECT * " +
								$"FROM {Table} ";

			return Read<T>(commandText, columns);
		}

		public void Add(int imageId, int markerId, string marker)
		{
			string commandText = $@"EXEC [dbo].Add{marker} {imageId}, {markerId}";

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
	}
}
