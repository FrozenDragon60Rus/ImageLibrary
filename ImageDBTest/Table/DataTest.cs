using ImageDB.Table;

namespace ImageDBTest.Table
{
    public class DataTest : IDisposable
    {
        private Data TestData { set; get; }
        public DataTest()
        {
            TestData = new Data();
            TestData.parameter.Add("Id", 1);
            TestData.parameter.Add("Name", "Test");
        }

        [Fact]
        public void DataGetParameterTest()
        {
            int id = Convert.ToInt32(TestData.parameter["Id"]);

            Assert.Equal(1, id);
        }
        [Fact]
        public void DataGetParameterKeyTest()
        {
            var key = TestData.parameter.Keys;

            Assert.Contains("Id", key);
        }
        [Fact]
        public void DataGetParameterNameTest()
        {
            string[] name = TestData.ParameterName();

            Assert.Equal(["Id", "Name"], name);
        }
        public void Dispose()
        {
            TestData = null;
        }
    }
}
