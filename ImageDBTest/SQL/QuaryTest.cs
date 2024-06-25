using ImageDB.SQL;


namespace ImageDBTest.SQL
{
#pragma warning disable CA1861
    public class QuaryTest
    {
        //<-------------------------------------Column test------------------------------------->
        [Theory,
            InlineData(
                new string[] { "first" }, 
                "first" 
            ),
            InlineData(
                new string[] { "first", "second", "third" }, 
                "first,second,third"
            )
        ]
        public void ColumnTest(string[] columns, string join) =>
            Assert.Equal(join, Quary.Column(columns));
        [Theory,
            InlineData([ new string[] { "" } ]),
            InlineData([ new string[] { "first", "", "third" } ])
        ]
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
            InlineData(
                new string[] { "first" }, 
                "@first"
            ),
            InlineData(
                new string[] { "first", "second", "third" }, 
                "@first,@second,@third"
            )
        ]
        public void ValueTest(string[] columns, string join) =>
            Assert.Equal(join, Quary.Value(columns));

        [Theory,
            InlineData([ new string[] { "" } ]),
            InlineData([ new string[] { "first", "", "third" } ])]
        public void ValueExceptionTest(string[] columns)
        {
            Exception e = Record.Exception(() => Quary.Column(columns));

            Assert.Equal("Can't be empty", e.Message);
        }
    }
}
