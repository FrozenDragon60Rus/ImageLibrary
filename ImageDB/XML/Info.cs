using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ImageDB.XML
{
    internal class Info
    {
        static private string name = "info.xml";
        static public string db
        {
            set
            {
                if (File.Exists(name))
                {
                    XDocument d = XDocument.Load(name);
                    d.Element("BaseInfo").Element("DBName").Value = value;
                    d.Save(name);
                    return;
                }
                WriteFolder();
            }
            get
            {
                if (File.Exists(name))
                    return XDocument.Load(name)
                                    .Element("BaseInfo")
                                    .Element("DBName")
                                    .Value;
                return Path.GetFullPath(@"%LOCALAPPDATA%\MyImageLibrary\DB\image.db");
            }
        }
        static public string folder
        {
            set
            {
                if (File.Exists(name))
                {
                    XDocument d = XDocument.Load(name);
                    d.Element("BaseInfo").Element("Folder").Value = value;
                    d.Save(name);
                    return;
                }
                WriteFolder();
            }
            get
            {
                if (File.Exists(name))
                    return XDocument.Load(name)
                                    .Element("BaseInfo")
                                    .Element("Folder")
                                    .Value;
                return string.Empty;
            }

        }

        static public void WriteFolder()
        {
                XDocument d = new XDocument();//XDocument.Load(name);
                XElement baseInfo = new XElement("BaseInfo"); //d.Element();
                baseInfo.Add(new XElement("Folder"));
                baseInfo.Add(new XElement("DBName"));
                d.Add(baseInfo);
                d.Save(name);
        }
    }
}
