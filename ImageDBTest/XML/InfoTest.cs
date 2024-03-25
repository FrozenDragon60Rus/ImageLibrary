using ImageDB.XML;

namespace ImageDBTest.XML
{
    
    public class InfoTest
    {


        [Fact]
        public void GetDBText()
        {

        }
        [Fact]
        public void GetFolderText()
        {
            Assert.Equal(@"I:\Яндекс диск\Pictures\Test", Info.Folder);
        }
    }
}
