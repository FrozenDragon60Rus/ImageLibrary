using System.IO;
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
                XDocument d2 = new XDocument(
                    new XElement("BaseInfo",
                        new XElement("DBName", value))
                );
                d2.Save(name);
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
                XDocument d2 = new XDocument(
                    new XElement("BaseInfo",
                        new XElement("Folder", value))
                );
                d2.Save(name);
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

        static public void writeFolder(string folder)
        {
            XDocument d = XDocument.Load(name);
            XElement baseInfo = d.Element("BaseInfo");
            baseInfo.Add(new XElement("folder", folder));
            d.Save(name);
        }
    }
}
