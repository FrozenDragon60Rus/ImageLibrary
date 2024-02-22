using ImageLibrary.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Runtime.CompilerServices;

namespace ImageLibrary
{
    internal class TagButton
    {
        public double Width;
        SearchParameter searchParameter;
        public void Fill(Grid TagsGroup, double Width, ref SearchParameter searchParameter)
        {
            this.Width = Width;
            this.searchParameter = searchParameter;

            DataBase tagDB = new DataBase("ImageLibrary", "Tag");
            List<string> tag = tagDB.Load("Name");

            //Array.Sort(tag, StringComparer.InvariantCulture);

            for (int i = 0; i < tag.Count; i++)
                TagsGroup.Children.Add(Create(tag[i], i));
        }

        private Button Create(string name, int num)
        {
            Button b = new Button()
            {
                Name = name + "_button",
                Content = name,
                Background = Brushes.White,
                FontSize = 11f,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 250,
                Height = 30
            };
            int imageOnLine = (int)Math.Truncate(Width / b.Width),
                row = (int)Math.Truncate((double)num / imageOnLine),
                col = num % imageOnLine;

            b.Margin = new Thickness(5 * (col + 1) + b.Width * col,
                                     5 * (row + 1) + b.Height * row,
                                     0, 0);
            b.Click += Check_tag;
            return b;

        }
        private void FillTagButton()
        {
            
        }
        private void Check_tag(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string item = b.Content.ToString();
            if (b.Background == Brushes.White)
            {
                searchParameter.tag.Add(item);
                b.Background = Brushes.Gray;
            }
            else
            {
                searchParameter.tag.Remove(item);
                b.Background = Brushes.White;
            }
        }
    }
}
