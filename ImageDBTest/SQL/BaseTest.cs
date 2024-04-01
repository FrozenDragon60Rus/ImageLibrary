using System.Data;
using ImageDB.SQL;

namespace ImageDBTest.SQL
{
    public class DataBaseTest : IDisposable
    {
        DataBase DB;
        public DataBaseTest() =>
            DB = new DataBase("ImageLibrary", "Image");

        [Fact]
        public void IsConnectionTest() =>
            Assert.Equal(ConnectionState.Closed, DB.State);
        public void Dispose() =>
            GC.SuppressFinalize(this);
    }
}
