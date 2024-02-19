using System.Collections.Generic;
using System.Linq;

namespace ImageDB
{
    internal class MyConvert
    {
        static public Dictionary<string, object> ArrayToDictionary(string[] key, object[] value) =>
        key.Zip(value, (s, o) => new { s, o })
                .ToDictionary(k => k.s, v => v.o);
    }
}
