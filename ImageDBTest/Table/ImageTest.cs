using ImageDB.Table;

namespace ImageDBTest.Table
{
    public class ImageTest : IDisposable
    {
        private Image TestData { set; get; }
        public ImageTest() =>
            TestData = Image.Empty;

        [Theory, InlineData(-1, "", 0)]
        public void ImageGetEmptyTest(int id, string address, int Rating)
        {
            Assert.Equal(id, TestData.Id);
            Assert.Equal(address, TestData.Address);
            Assert.Equal(Rating, TestData.Rating);
        }
        [Fact]
        public void ImageEmptySameTest()
        {
            var data = new Image(string.Empty);

            Assert.Same(TestData, data);
        }
        public void Dispose() =>
            GC.SuppressFinalize(this);
    }
}
