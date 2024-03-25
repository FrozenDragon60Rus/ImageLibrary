using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ImageDBTest")]
namespace ImageDB
{
    internal class MyConvert
    {
        static public Dictionary<string, object> ArrayToDictionary(string[] key, object[] value)
        {
            if (value is null || 
                key.Contains(string.Empty))
                throw new Exception("Key or Value must not be empty",
                                    new Exception());
            if (key.Length != value.Length)
                throw new Exception("Key and Value length must be same",
                                    new Exception());

            return key.Zip(value, (s, o) => new { s, o })
                      .ToDictionary(k => k.s, v => v.o);
        }
    }
}
