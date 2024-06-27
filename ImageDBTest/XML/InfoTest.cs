using ImageDB.XML;

namespace ImageDBTest.XML
{
    public class InfoTest : IDisposable
    {
        private readonly string dbName,
                                Folder;

        public InfoTest()
        {
            dbName = Info.DB.Clone().ToString() ?? string.Empty;
			Folder = Info.Folder.Clone().ToString() ?? string.Empty;
        }
        [Fact]
        public void InfoDBTest()
        {
            Info.DB = @"%appdatalocal%\MyImageLibrary";

            Assert.Equal(@"%appdatalocal%\MyImageLibrary", Info.DB);

            Info.DB = dbName;
        }

		[Fact]
		public void InfoFolderTest()
		{
			Info.Folder = @"MyImageLibrary";

			Assert.Equal(@"MyImageLibrary", Info.Folder);

            Info.Folder = Folder;
		}

        public void Dispose()
        {
            Info.DB = dbName;
            Info.Folder = Folder;
            GC.SuppressFinalize(this);
        }
    }
}
