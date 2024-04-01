using ImageDB.XML;

namespace ImageDBTest.XML
{
    public class InfoTest : IDisposable
    {
        public InfoTest() =>
            Info.DB = string.Empty;
        [Fact]
        public void SetDBTest()
        {
            Info.DB = @"%appdatalocal%\MyImageLibrary";

            Assert.Equal(@"%appdatalocal%\MyImageLibrary", Info.DB);
        }
        [Fact]
        public void GetDBTest()
        {

        }
        [Fact]
        public void GetFolderTest()
        {
            Assert.Equal(@"I:\Яндекс диск\Pictures\Test", Info.Folder);
        }

        public void Dispose() =>
            GC.SuppressFinalize(this);
    }
}
