using System.Data;
using ImageDB.Table;
using ImageDB.SQL;

namespace ImageDBTest.SQL
{
    public class DataBaseTest : IDisposable
    {
        private DataBase DB { get; }

        public DataBaseTest()
        {
            DB = new DataBase("ImageLibrary", "Test");
        } 

        [Fact]
        public void IsConnectionTest() =>
            Assert.Equal(ConnectionState.Closed, DB.State);
		[Fact]
        public void AddLoadTest()
        {
            Image img = new(1, "no", 1);

            DB.Add(img);
            var result = DB.Load<Image>();

			Assert.Equal(img, result.Last());
        }
        public void Dispose()
        {
            DB.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
