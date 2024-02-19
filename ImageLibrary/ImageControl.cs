using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using ImageLibrary.SQL;

namespace ImageLibrary
{
    internal class ImageControl
    {
        public Image[] Images;
        private Grid Viewer;
        DataBase dataBase;
        public readonly int imageCount; 
        public ImageControl(ref Grid viewer)
        {
            Viewer = viewer;
            dataBase = new DataBase("ImageLibrary", "dbo.Image");
            imageCount = dataBase.GetRowCount();
        }
        static public Vector Resize(Vector vector1, Vector vector2)
        {
            Vector vector = new Vector();
            double percent;

            if (vector2.X > vector2.Y)
            {
                percent = vector1.Y / vector1.X;
                vector.X = vector2.X;
                vector.Y = vector.X * percent;

                if (vector.Y > vector2.Y)
                {
                    percent = vector.X / vector.Y;
                    vector.Y = vector2.Y;
                    vector.X = vector.Y * percent;
                }
                return vector;
            }

            percent = vector1.X / vector1.Y;
            vector.Y = vector2.Y;
            vector.X = vector.Y * percent;

            if (vector.X > vector2.X)
            {
                percent = vector.Y / vector.X;
                vector.X = vector2.X;
                vector.Y = vector.X * percent;
            }
            return vector;
        }

        public void Draw(int num, Vector size)
        {
            Image img = new Image(), 
                  bg = new Image();

            img.Source = Images[num].Source;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(Directory.GetCurrentDirectory() + @"\bg.png");
            bi.EndInit();
            bg.Source = bi;
            bg.Width = size.X;
            bg.Height = size.Y;

            Vector imageSize = Resize(new Vector(img.Source.Width, img.Source.Height), new Vector(size.X, size.Y));
            img.Stretch = Stretch.UniformToFill;
            img.Width = imageSize.X;
            img.Height = imageSize.Y;
            img.VerticalAlignment = VerticalAlignment.Center;
            img.HorizontalAlignment = HorizontalAlignment.Center;
            img.MouseUp += ImageClose;

            Viewer.Children.Add(bg);
            Viewer.Children.Add(img);
        }
        public void Load(int imageCount, int currentPage, Vector size)
        {
            int offset = currentPage * 10,
                count = offset + 10 > imageCount ? imageCount - offset
                                                 : offset + 10; ;
            List<string> imageName = dataBase.Load(offset, count);
            for (int i = 0; i < imageName.Count; i++)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                Console.WriteLine(imageName[i]);
                bi.UriSource = new Uri(imageName[i]);
                try
                {
                    bi.EndInit();
                }
                catch
                {
                    Console.WriteLine("Image " + Path.GetFileName(imageName[i]) + " not found");
                    bi = ToBitmapImage(Resource.no_foto);
                }
                Vector s = Resize(new Vector(bi.PixelWidth, bi.PixelHeight), size);
                Images[i].Source = bi;
                //Console.WriteLine("IX-" + s.X + " IY-" + s.Y);
                Images[i].Width = s.X;
                Images[i].Height = s.Y;
            }
            if (imageCount < Images.Length)
                for (int i = imageCount; i < Images.Length; i++)
                {
                    Images[i].Width = 0;
                    Images[i].Height = 0;
                }
        }
        public BitmapImage ToBitmapImage(System.Drawing.Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            ms.Close();
            return image;
        }

        private void ImageClose(object sender, MouseButtonEventArgs e)
        {
            Viewer.Children.Clear();
        }
    }
}
