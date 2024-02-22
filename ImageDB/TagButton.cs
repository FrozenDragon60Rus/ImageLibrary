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

namespace ImageDB
{
    internal class TagButton
    {
        string Name;
        DB form;

        public TagButton(DB form, string Name)
        {
            this.form = form;
            this.Name = Name;
        }

        public void Fill()
        {
            DataBase tag = new DataBase("ImageLibrary", Name);
            List<Table.Marker> marker = new List<Table.Marker>();

            tag.Load(ref marker);

            foreach (Table.Marker Tag in marker)
                form.TagsGroup.Children.Add(Create(Tag.Name, Tag.Id));
        }
        public Button Create(string name, int num)
        {
            Button b = new Button()
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
            b.Margin = new Thickness(5,
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
            DataBase marker = new DataBase("ImageLibrary", "ImageBy" + Name);

            marker.Add((int)form.currentItem.parameter["Id"], (int)TagButton.Tag, Name);
            form.currentItem.parameter = form.dataBase
                                             .LoadById((int)form.currentItem.parameter["Id"], form.join);

            form.ImageData.Items.Refresh();
        }
        public void RemoveTagEvent(object sender, EventArgs e)
        {
            Button TagButton = sender as Button;
            DataBase marker = new DataBase("ImageLibrary", "ImageBy" + Name);

            marker.Delete((int)form.currentItem.parameter["Id"], (int)TagButton.Tag);

            form.ImageData.Items.Refresh();
        }
    }
}
