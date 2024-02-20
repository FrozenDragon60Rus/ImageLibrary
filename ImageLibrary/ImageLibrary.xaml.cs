using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using ImageLibrary.SQL;
using ImageLibrary;

namespace MyImageLibrary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ImageLibrary : Window
    {
        public short imageOnPage = 10,
                     imageRow = 2;
        private int MaxImage = 10;

        ImageControl imageControl;
        SearchParameter searchParameter;
        public Vector imageSize
        {
            get
            {
                double x, y, imageMarginX = 4, imageMarginY = 4;
                int countOnLine = (int)Math.Floor((float)(imageOnPage / imageRow));
                x = (BorderGroup.Width / countOnLine - (imageMarginX * (countOnLine - 1)));
                y = (BorderGroup.Height / imageRow - (imageMarginY * (imageRow - 1)));
                return new Vector(x, y);
            }
        }
        private int currentPage
        {
            set {
                if (value < 0)
                    page = 0;
                else if ((value * imageOnPage) > imageControl.imageCount)
                    return;
                page = value;
            }
            get => page;
        }
        private int page = 0;

        public ImageLibrary()
        {
            InitializeComponent();
        }

        private void LeftPage_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void LeftPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Left_" + currentPage);
            currentPage--;
            imageControl.Load(imageOnPage, 
                              currentPage,
                              imageSize);
        }

        private void RightPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Right_" + currentPage);
            currentPage++;
            imageControl.Load(imageOnPage, 
                              currentPage, 
                              imageSize);
        }
        

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // @"F:\ЯДиск\C#\MyImageLibrary\MyImageLibrary\bin\Debug\DB\furry.db"
                try
                {                    
                    imageControl.Load(imageOnPage, currentPage, imageSize);
                }
                catch (IOException)
                {
                    Console.WriteLine("Файл занят ");                           
                }
                currentPage = 0;
            }
        }

        private void FillImageGrid()
        {
            int countOnLine = (int)Math.Floor((float)(imageOnPage / imageRow));
            double imageMarginX = 2, imageMarginY = 2;
            imageControl.Images = new Image[imageOnPage];
            Vector size = imageSize;
            Image img;

            for (int i = 0; i < imageOnPage; i++)
            {
                int row = i % countOnLine,
                    col = i % imageRow,
                    rowOffset = (int)(row * size.X + (row * imageMarginX)),
                    colOffset = (int)(col * size.Y + (col * imageMarginY));
                //Console.WriteLine(i + ") " + rowOffset + " " + colOffset);

                img = new Image()
                {
                    Margin = new Thickness(rowOffset,
                                           colOffset,
                                           imageMarginX,
                                           imageMarginY),
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Tag = i
                };
                img.MouseUp += PreviewImageClick;
                imageControl.Images[i] = img;
                ImageGroup.Children.Add(imageControl.Images[i]);
            }
            imageControl.Load(imageOnPage, currentPage, imageSize);
            //Console.WriteLine("end");
        }

        private void PreviewImageClick(object sender, MouseButtonEventArgs e)
        {
            Viewer.Children.Clear();
            Image img = sender as Image;
            int id = (int)img.Tag;
            imageControl.Draw(id, new Vector(Width, Height*0.95));
        }
        private void ImageClose(object sender, MouseButtonEventArgs e)
        {
            Viewer.Children.Clear();
        }
        private Button TagButton(string name, int num)
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
            int imageOnLine = (int)Math.Truncate(TagsBox.Width / b.Width),
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
            DataBase tagDB = new DataBase("ImageLibrary", "Tag");
            List<string> tag = tagDB.Load("Name");

            //Array.Sort(tag, StringComparer.InvariantCulture);

            for (int i = 0; i < tag.Count; i++)
                TagsGroup.Children.Add(TagButton(tag[i], i));
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

        private void ShowTags_Click(object sender, RoutedEventArgs e)
        {
            if (BorderGroup.Visibility == Visibility.Visible)
            {
                BorderGroup.Visibility = Visibility.Hidden;
                TagsBox.Visibility = Visibility.Visible;
                return;
            }
            BorderGroup.Visibility = Visibility.Visible;
            TagsBox.Visibility = Visibility.Hidden;

            try
            {
                currentPage = 0;
                imageControl.Load(imageOnPage, currentPage, imageSize);
            }
            catch (IOException)
            {
                Console.WriteLine("Файл занят ");
            }
        }

        private void GroupSize()
        {
            BorderGroup.Height = TagsBox.Height = Height - 100;
            BorderGroup.Width = TagsBox.Width = Width - 26;
            //Console.WriteLine("width(" + Width + ") height(" + Height + ")");
            FindImage.Width = ShowTags.Width = Width - 150;
            Viewer.Width = Main.Width;
            Viewer.Height = Main.Height;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            imageControl = new ImageControl(ref Viewer);
            GroupSize();
            FillImageGrid();
            FillTagButton();
            Console.WriteLine(MaxImage);
        }
    }
}
