using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

namespace ImageLibrary
{
    internal class ImageControl
    {
        public Image[] Images;
        private Grid Viewer { get; }
        public ImageControl(ref Grid viewer) =>
            Viewer = viewer;
        static public Vector Resize(Vector vector1, Vector vector2)
        {
            Vector vector = new();
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
            Image img, bg;

            BitmapImage bi = new();
            bi.BeginInit();
            bi.UriSource = new Uri(Directory.GetCurrentDirectory() + @"\bg.png");
            bi.EndInit();

            bg = new Image()
            {
                Source = bi,
                Width = size.X,
                Height = size.Y
            };

            Vector imageSize = Resize(new Vector(Images[num].Source.Width, Images[num].Source.Height),
                                      new Vector(size.X, size.Y));
            img = new Image()
            {
                Source = Images[num].Source,
                Stretch = Stretch.UniformToFill,
                Width = imageSize.X,
                Height = imageSize.Y,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            }; img.MouseUp += ImageClose;

            Viewer.Children.Add(bg);
            Viewer.Children.Add(img);
        }
        public void Load(Vector size, IEnumerable<string> imageName)
        {
            int index = 0;
            foreach(var name in imageName)
            {
                //Debug.WriteLine(name);
				BitmapImage bi = new();
				bi.BeginInit();
				bi.UriSource = new Uri(name);
				try
				{
					bi.EndInit();
				}
				catch
				{
					Console.WriteLine("Image " + Path.GetFileName(name) + " not found");
					bi = ToBitmapImage(Resource.no_foto);
				}
				Vector s = Resize(new Vector(bi.PixelWidth, bi.PixelHeight), size);
				Images[index].Source = bi;
				//Console.WriteLine("IX-" + s.X + " IY-" + s.Y);
				Images[index].Width = s.X;
				Images[index].Height = s.Y;
				Images[index++].Visibility = Visibility.Visible;
			}
            if (imageName.Count() < Images.Length)
                for (int i = imageName.Count(); i < Images.Length; i++)
                    Images[i].Visibility = Visibility.Hidden;
        }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
		private BitmapImage ToBitmapImage(System.Drawing.Bitmap src)
        {
            var ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new();
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
