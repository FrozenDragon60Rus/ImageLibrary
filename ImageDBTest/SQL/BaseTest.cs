using System.Data;
using ImageDB.Table;
using ImageDB.SQL;

namespace ImageDBTest.SQL
{
    public class BaseTest : IDisposable
    {
        private ImageDataBase DB { get; }

        public BaseTest()
        {
            DB = new ImageDataBase("ImageLibrary", "Test", []);
        } 

        [Fact]
        public void IsConnectionTest() =>
            Assert.Equal(ConnectionState.Closed, DB.State);

        public void Dispose()
        {
            DB.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
