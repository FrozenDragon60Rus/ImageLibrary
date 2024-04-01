using System.IO;

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
