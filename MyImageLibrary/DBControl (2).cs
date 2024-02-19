using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ImageDB
{
    static class DBControl
    {
        private const string fileName = "";
        static public void Load(ref List<DBTable> table)
        {
            BinaryReader r = new BinaryReader(new FileStream(@"DB\furry.db", FileMode.Open, FileAccess.Read));
            table.Clear();
            DBTable column = new DBTable();
            table.Add(r.ReadUInt32());
            r.Close();
        }
        static public void FormFromFolder(ref List<DBTable> table, string folderName)
        {
            string[] files = Directory.GetFiles(folderName);
            string directory = Directory.GetCurrentDirectory() + @"\DB";
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            Console.WriteLine(directory);
            BinaryWriter w = new BinaryWriter(File.Create(@"DB\"+Path.GetFileName(folderName)+".db"));
            DBTable Column;
            foreach (string file in files)
            {
                Column = new DBTable(file);
                table.Add(Column);
                w.Write(Column.ID);
                w.Write(Column.Name);
                w.Write(Column.Tag);
            }
            
            w.Close();
            Load(ref table);
        }
        static public void TestLoad(ref List<DBTable> table, string str)
        {
            StreamReader SR = new StreamReader(str);
            string[] s = SR.ReadLine().Split(',');
            string name = s[0], tag = s[1];
            table.Add(new DBTable(name, tag));
            SR.Close();
        }
        static public void Save()
        {

        }
        static public void TestSave(string str)
        {
            StreamWriter SW = new StreamWriter(new FileStream(str, FileMode.Create));
            string name = "1111.jpg", tag = "";
            SW.WriteLine(name + "," + tag);
            SW.Close();
        }
        static public List<DBTable> Fill()
        {
            string folder = "";
            string[] files;
            files = Directory.GetFiles(folder, null, SearchOption.AllDirectories);
            List<DBTable> table = new List<DBTable>();
            foreach (string f in files)
                table.Add(new DBTable(f));
            return table;
        }
    }

    public class DBTable
    {
        private static uint id = 0;
        public DBTable(string Name, string Tag)
        {
            this.Name = Name;
            this.Tag = Tag;
            this.ID = id++;
        }
        public DBTable(string Name)
        {
            this.Name = Name;
            this.Tag = "";
            this.ID = 0;
        }
        public string Name { set; get; }
        public string Tag { set; get; }
        public uint ID { get; }
    }
}
