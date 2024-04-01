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
        [Theory, InlineData("", "", "")]
        public void ImageCheckAdditionalDataTest(string tag, string character, string author)
        {
            Assert.Equal(tag, TestData.Tag);
            Assert.Equal(character, TestData.Character);
            Assert.Equal(author, TestData.Author);
        }
        [Fact]
        public void ImageIsEmptyTest()
        {
            Image data = new(string.Empty);

            Assert.Equal(data, TestData);
        }
        public void Dispose() =>
            GC.SuppressFinalize(this);
    }
}
