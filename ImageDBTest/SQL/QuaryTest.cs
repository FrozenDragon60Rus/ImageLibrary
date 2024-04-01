using System.Collections;
using System.Runtime.CompilerServices;
using ImageDB.SQL;


namespace ImageDBTest.SQL
{
#pragma warning disable CA1861
    public class QuaryTest
    {
        //<-------------------------------------Column test------------------------------------->
        [Theory,
            InlineData(new string[] { "first" }, 
                       "first" ),
            InlineData(new string[] { "first", "second", "third" }, 
                       "first,second,third")]
        public void ColumnTest(string[] columns, string join) =>
            Assert.Equal(join, Quary.Column(columns));
        [Theory,
            InlineData(new object[] {
                            new string[] { "" } }),
            InlineData(new object[] {
                            new string[] { "first", "", "third" } })]
        public void ColumnExceptionTest(string[] columns)
        {
            Exception e = Record.Exception(() => Quary.Column(columns));

            Assert.Equal("Can't be empty", e.Message);
        }
        //<-------------------------------------Value test------------------------------------->
        [Fact]
        public void KeysInValueHasntChangedTest()
        {
            string[] keys = ["first", "second", "third"];
            string[] copy = (string[]) keys.Clone();
            string join = Quary.Value(keys);
            Assert.Equal(keys, copy);
        }
        [Theory,
            InlineData(new string[] { "first" }, 
                       "@first"),
            InlineData(new string[] { "first", "second", "third" }, 
                       "@first,@second,@third")]
        public void ValueTest(string[] columns, string join) =>
            Assert.Equal(join, Quary.Value(columns));

        [Theory,
            InlineData(new object[] {
                            new string[] { "" } }),
            InlineData(new object[] {
                            new string[] { "first", "", "third" } })]
        public void ValueExceptionTest(string[] columns)
        {
            Exception e = Record.Exception(() => Quary.Column(columns));

            Assert.Equal("Can't be empty", e.Message);
        }
        //<-------------------------------------ColumnWithValue test------------------------------------->

        [Theory,
            InlineData(new string[] { "first" }, 
                       "first=@newfirst"),
            InlineData(new string[] { "first", "second", "third" }, 
                       "first=@newfirst,second=@newsecond,third=@newthird")]
        public void AssignValueToColumnTest(string[] columns, string join) =>
            Assert.Equal(join, Quary.AssignValueToColumn(columns));
        [Theory,
            InlineData(new object[] {
                            new string[] { "" } }),
            InlineData(new object[] {
                            new string[] { "first", "", "third" } })]
        public void AssignValueToColumnExceptionTest(string[] columns)
        {
            Exception e = Record.Exception(() => Quary.Column(columns));

            Assert.Equal("Can't be empty", e.Message);
        }

        //<-------------------------------------Join test------------------------------------->
        [Fact]
        public void JoinTest()
        {
            string joinText = "\r\nLEFT JOIN( \r\n" +
                                   $"SELECT ImageByTag.Image_Id, STRING_AGG(CAST(Tag.Name AS VARCHAR(1024)), ',') AS Tag \r\n" +
                                   $"FROM ImageByTag \r\n" +
                                   $"LEFT JOIN Tag \r\n" +
                                   $"ON ImageByTag.Tag_Id = Tag.Id \r\n" +
                                   $"GROUP BY ImageByTag.Image_Id \r\n" +
                              $") AS JoinTag \r\n" +
                              $"ON Image.Id = JoinTag.Image_Id\r\n";

            Assert.Equal(joinText, Quary.Join("Image", ["Tag"]));
        }
    }
}
