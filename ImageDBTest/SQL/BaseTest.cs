using System.Data;
using ImageDB.SQL;

namespace ImageDBTest.SQL
{
    public class DataBaseTest(DataBase DB) : IDisposable
    {
        private DataBase DB { get; } = DB;

        [Fact]
        public void IsConnectionTest() =>
            Assert.Equal(ConnectionState.Closed, DB.State);
        public void Dispose() =>
            GC.SuppressFinalize(this);
    }
}
