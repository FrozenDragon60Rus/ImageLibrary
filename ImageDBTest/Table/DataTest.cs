using ImageDB.Table;

namespace ImageDBTest.Table
{
    public class DataTest : IDisposable
    {
        private Data TestData { set; get; }
        public DataTest()
        {
            TestData = new Data();
            TestData.Parameter.Add("Id", 1);
            TestData.Parameter.Add("Name", "Test");
        }

        [Fact]
        public void DataGetParameterTest()
        {
            int id = Convert.ToInt32(TestData.Parameter["Id"]);

            Assert.Equal(1, id);
        }
        [Fact]
        public void DataGetParameterKeyTest()
        {
            var key = TestData.Parameter.Keys;

            Assert.Contains("Id", key);
        }
        public void Dispose() =>
            GC.SuppressFinalize(this);
    }
}
