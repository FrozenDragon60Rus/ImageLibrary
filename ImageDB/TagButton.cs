using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using ImageDB.SQL;
using System.Xml.Linq;
using System.Runtime.Versioning;

namespace ImageDB
{
    [SupportedOSPlatform("Windows")]
    internal class TagButton (DB Form, string Name)
    {
        string Name { get; } = Name;
        private DB Form { get; } = Form;

        public void Fill()
        {
            DataBase tag = new("ImageLibrary", Name);
            List<Table.Marker> marker = [];

            tag.Load(ref marker);

            foreach (Table.Marker Tag in marker)
                Form.TagsGroup.Children.Add(Create(Tag.Name, Tag.Id));
        }
        public Button Create(string name, int num)
        {
            Button b = new()
            {
                Name = name + "_button",
                Content = name,
                Background = Brushes.White,
                FontSize = 16f,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 250,
                Height = 30,
                Tag = num
            };
            b.Margin = new(5,
                          (5 + b.Height) * (num - 1),
                           0,
                           0);
            b.Click += AddTagEvent;
            b.MouseRightButtonUp += RemoveTagEvent;

            return b;
        }

        public void AddTagEvent(object sender, EventArgs e)
        {
            Button TagButton = sender as Button;
            DataBase marker = new("ImageLibrary", "ImageBy" + Name);

            marker.Add(Form.currentItem.Id, (int)TagButton.Tag, Name);
            Form.currentItem.parameter = Form.dataBase
                                             .LoadById(Form.currentItem.Id, Form.join);

            Form.ImageData.Items.Refresh();
        }
        public void RemoveTagEvent(object sender, EventArgs e)
        {
            Button TagButton = sender as Button;
            DataBase marker = new("ImageLibrary", "ImageBy" + Name);

            marker.Delete(Form.currentItem.Id, (int)TagButton.Tag);

            Form.ImageData.Items.Refresh();
        }
    }
}
