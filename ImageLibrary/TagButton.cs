using ImageLibrary.SQL;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Diagnostics;

namespace ImageLibrary
{
    internal class TagButton
    {
        public double Width;
        Filter filter;
        public void Fill(Grid TagsGroup, double Width, ref Filter filter)
        {
            this.Width = Width;
            this.filter = filter;

            var tagDB = new DataBase("ImageLibrary", "Tag");
            var tag = tagDB.Load("Name");

            //Array.Sort(tag, StringComparer.InvariantCulture);

            for (int i = 0; i < tag.Count; i++)
                TagsGroup.Children.Add(Create(tag[i], i));
        }

        private Button Create(string name, int num)
        {
            var b = new Button()
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
        private void Check_tag(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string item = b.Content.ToString();
            if (b.Background == Brushes.White)
            {
				filter.Tag.Add(item);
                Debug.WriteLine(filter.Marker["Tag"].IndexOf(item));
                b.Background = Brushes.Gray;
            }
            else
            {
				filter.Tag.Remove(item);
                b.Background = Brushes.White;
            }
        }
    }
}
