using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using System.Xml.Linq;

namespace ImageDB.Binary
{
    internal class DataBase
    {
        string Name,
               Table;
        BinaryReader connection;
        public DataBase(string name, string table)
        {
            Name = name;
            Table = table;
            connection = Connect();
        }

        public BinaryReader Connect() =>
            new BinaryReader(new FileStream(Name, FileMode.Create, FileAccess.Write));
        public List<string> Load(int offset, int count)
        {
            List<string> list = new List<string>();

            return list;
        }
        public List<string> Load(string type)
        {
            List<string> list = new List<string>();

            return list;
        }

        /*public string[] ImagePath(int maxImage, int imageCount, int offset)
        {
            BinaryReader r = new BinaryReader(new FileStream(@"TagList", FileMode.Open, FileAccess.Read));
            maxImage = r.ReadInt32();
            if (maxImage < 1)
            {
                imageCount = 0;
                return new string[0];
            }
            //Console.WriteLine("MaxImage - " + MaxImage);
            path = r.ReadString();
            int offset = currentPage * imageOnPage;
            for (int i = 0; i < offset; i++)
            {
                //r.ReadString();
                r.ReadString();
            }
            imageCount = maxImage - offset >= imageOnPage ? imageOnPage 
                                                          : maxImage - offset;
            string[] str = new string[imageCount];
            Console.WriteLine("count_"+ imageCount);
            for(int i = 0; i < imageCount; i++)
            {
                str[i] = path + "\\" + r.ReadString();
                //Console.WriteLine(str[i]);
                //r.ReadString();
            }
            r.Close();
            return str;
        }*/
    }


}
