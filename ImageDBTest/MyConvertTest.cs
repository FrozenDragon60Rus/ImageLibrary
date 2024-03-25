using ImageDB;

namespace ImageDBTest
{
    public class MyConvertTest
    {
        [Theory,
            InlineData(new string[] { "1", "2" }, new object[] { 1 }),
            InlineData(new string[] { "1" }, new object[] { 1, 2 }),
            InlineData(new string[] { "" }, null)]
        public void ArrayToDictionaryExceptionTest(string[] key, object[] value)
        {
            Dictionary<string, object> testDictionary;

            Assert.Throws<Exception>(() => 
                testDictionary = MyConvert.ArrayToDictionary(key, value));
        }

        [Theory,
            InlineData(new string[] {"1"}, new object[] { 1 }),
            InlineData(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"}, 
                       new object[] { 1, 2, 3, 4, 5, 6.0, "7", 8, 9, false })]
        public void ArrayToDictionaryTest(string[] key, object[] value)
        {
            Dictionary<string, object> testDictionary;

            testDictionary = MyConvert.ArrayToDictionary(key, value);

            int index = 0;
            foreach (var k in key)
                Assert.Equal(testDictionary[k], value[index++]);
        }
    }
}