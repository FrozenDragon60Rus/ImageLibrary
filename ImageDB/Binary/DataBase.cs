using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using ImageDB.Table;
using System.Xml.Serialization;

namespace ImageDB.Binary
{
    static class DataBase
    {
        /*[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern int MessageBoxTimeout(IntPtr hwnd, String text, String title,
                                     uint type, Int16 wLanguageId, Int32 milliseconds);*/

        static public void Load(ref List<DBTable> table)
        {
            var r = new BinaryReader(new FileStream(@"DB\f.db", FileMode.Open, FileAccess.Read));
            for (int i = table.Count - 1; i >= 0; i--)
                table.RemoveAt(i);
            uint id = 0;
            DBTable column;
            r.ReadInt32();
            r.ReadString();
            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                column = new(r.ReadString())
                {
                    ID = id++,
                    Tag = r.ReadString(),
                    Raiting = r.ReadByte()
                };
                //Console.WriteLine(column.ID + ") " + column.Name + " \"" + column.Tag + " \"");
                table.Add(column);
            }
            r.Close();
            
        }
        static public void Load(ref List<DBTable> table, string fileName)
        {
            var r = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
            for (int i = table.Count - 1; i >= 0; i--)
                table.RemoveAt(i);
            uint id = 0;
            DBTable column;
            r.ReadInt32();
            r.ReadString();

            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                column = new DBTable(r.ReadString())
                {
                    ID = id++,
                    Tag = r.ReadString(),
                    Raiting = r.ReadByte()
                };
                //Console.WriteLine(column.ID + ") " + column.Name + " \"" + column.Tag + " \"");
                table.Add(column);
            }
            r.Close();
        }
        static public void FromFolder(ref List<DBTable> table, string folderName)
        {
            string[] files = Directory.GetFiles(folderName);
            string directory = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\MyImageLibrary\DB\image.db"));
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            XML.Info.DB = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\MyImageLibrary\DB\image.db");
            //Console.WriteLine(directory);
            var w = new BinaryWriter(File.Create(XML.Info.DB));

            w.Write(files.Length);
            w.Write(Path.GetFullPath(folderName));
            foreach (string file in files)
            {
                w.Write(Path.GetFileName(file));
                w.Write(string.Empty);
                w.Write((byte)0);
            }
            
            w.Close();
            Load(ref table, XML.Info.DB);
        }
        static public void TestLoad(ref List<DBTable> table, string str)
        {
            var SR = new StreamReader(str);
            
            string[] s = SR.ReadLine().Split(',');
            string name = s[0], tag = s[1];
            table.Add(new DBTable(name, tag, 0, 0));
            SR.Close();
        }
        static public void Save(List<DBTable> table)
        {
            /*if(db == string.Empty)
            {
                Microsoft.Win32.SaveFileDialog SFD = new Microsoft.Win32.SaveFileDialog();
                SFD.FileName = "Image";
                SFD.DefaultExt = ".db";
                SFD.FileOk += SFD_FileOK;
                SFD.ShowDialog();
            }*/
            string ImageDirectory,
                   ImageDB = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\MyImageLibrary\DB\image.db"),
                   Temp = "DB\\temporary.db";
            using(var r = new BinaryReader(File.OpenRead(XML.Info.DB)))
                {
                    r.BaseStream.Position += 4;
                    ImageDirectory = r.ReadString();
                    r.Close();
                }
            var w = new BinaryWriter(File.Create(Temp));

            w.Write(table.Count);
            w.Write(ImageDirectory);
            foreach (DBTable info in table)
            {
                w.Write(info.Name);
                w.Write(info.Tag);
                w.Write(info.Raiting);
            }
            w.Close();

            File.Copy(Temp, ImageDB, true);
            File.Delete(Temp);

            //MessageBox.Show("База обновлена");
            //MessageBoxTimeout(IntPtr.Zero, "База обновлена", "БД", 0, 0, 500);

            XML.Info.DB = ImageDB;
        }

        static public void Refresh(ref List<DBTable> table)
        {
            string ImageDirectory;

            if (File.Exists(XML.Info.DB))
            {
                using (var r = new BinaryReader(File.OpenRead(XML.Info.DB)))
                {
                    r.BaseStream.Position += 4;
                    ImageDirectory = r.ReadString();

                    r.Close();
                }
                string[] files = Directory.GetFiles(ImageDirectory);

                DBTable img;
                string extension;
                foreach (string file in files)
                {
                    extension = Path.GetExtension(file);
                    if (extension == "mp4" || extension == "webm") continue;
                    if (table.Find(item => item.Name == Path.GetFileName(file)) == null)
                    {
                        img = new DBTable(Path.GetFileName(file));
                        table.Add(img);
                    }
                }
                table = [..table.OrderBy(row => row.Name)];
                Save(table);
            }
        }
        static private bool CheckExtension(string extension)
        {
            return extension switch
            {
                "jpg" => true,
                "png" => true,
                "bmp" => true,
                _ => false,
            };
        }
        static public void FindEmpty(List<DBTable> table)
        {
            foreach(DBTable t in table)
                if (t.Tag == string.Empty) Console.WriteLine(t.ID + " is empty");
        }

        static private void SFD_FileOK(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*Microsoft.Win32.SaveFileDialog SFD = sender as Microsoft.Win32.SaveFileDialog;
            if (SFD.FileNames.Length > 0) db = SFD.FileName;
            else XMLInfo.db = @"DB\Image.db";   */
        }

        static public List<DBTable> Fill()
        {
            string folder = "";
            string[] files;
            files = Directory.GetFiles(folder, null, SearchOption.AllDirectories);
            List<DBTable> table = [];
            foreach (string f in files)
                table.Add(new DBTable(f));
            return table;
        }
    }

    public class DBTable
    {
        public DBTable(string Name, string Tag, uint id, byte rate)
        {
            this.Name = Name;
            this.Tag = Tag;
            ID = id;
            Raiting = rate;
        }
        public DBTable(string Name)
        {
            this.Name = Name;
            Tag = String.Empty;
            ID = 0;
            Raiting = 0;
        }
        public string Name { set; get; }
        public string Tag { set; get; }
        public uint ID { set; get; }
        public byte Raiting { set; get; }
    }
}
