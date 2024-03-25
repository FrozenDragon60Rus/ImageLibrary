using System.IO;
using System.Xml.Linq;

namespace ImageDB.XML
{
    internal class Info
    {
        static private string Name { get; } = "info.xml";
        static public string DB
        {
            set
            {
                if (File.Exists(Name))
                {
                    var d = XDocument.Load(Name);
                    d.Element("BaseInfo").Element("DBName").Value = value;
                    d.Save(Name);
                    return;
                }
                WriteFolder();
            }
            get
            {
                if (File.Exists(Name))
                    return XDocument.Load(Name)
                                    .Element("BaseInfo")
                                    .Element("DBName")
                                    .Value;
                return Path.GetFullPath(@"%LOCALAPPDATA%\MyImageLibrary\DB\image.db");
            }
        }
        static public string Folder
        {
            set
            {
                if (File.Exists(Name))
                {
                    var d = XDocument.Load(Name);
                    d.Element("BaseInfo").Element("Folder").Value = value;
                    d.Save(Name);
                    return;
                }
                WriteFolder();
            }
            get
            {
                if (File.Exists(Name))
                    return XDocument.Load(Name)
                                    .Element("BaseInfo")
                                    .Element("Folder")
                                    .Value;
                return string.Empty;
            }

        }

        static public void WriteFolder()
        {
                var d = new XDocument();//XDocument.Load(name);
                var baseInfo = new XElement("BaseInfo"); //d.Element();
                baseInfo.Add(new XElement("Folder"));
                baseInfo.Add(new XElement("DBName"));
                d.Add(baseInfo);
                d.Save(Name);
        }
    }
}
