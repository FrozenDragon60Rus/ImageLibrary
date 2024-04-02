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
        private readonly short imageOnPage = 10,
                               imageRow = 2;
        private readonly int MaxImage = 10;

        ImageControl imageControl;
        Filter filter = new();
        public Vector ImageSize
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
        private int CurrentPage
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

        private void FillImageGrid()
        {
            int countOnLine = (int)Math.Floor((float)(imageOnPage / imageRow));
            double imageMarginX = 2, imageMarginY = 2;
            imageControl.Images = new Image[imageOnPage];
            Vector size = ImageSize;
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
            //imageControl.Load(imageOnPage, CurrentPage, ImageSize, filter);
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
            new TagButton().Fill(TagsGroup, TagsBox.Width, ref filter);
            Console.WriteLine(MaxImage);
        }
    }
}
