using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Effects;

namespace ImageDB.SQL
{
    internal static class Convert
    {
        public static void FromBinaryToSQL(string binary, string sql)
        {
            using (BinaryReader binaryDB = new BinaryReader(File.OpenRead(binary)))
            {

            }
        }
    }
}
