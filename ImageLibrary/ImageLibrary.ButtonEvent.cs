using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyImageLibrary
{
	public partial class ImageLibrary : Window
	{
		private void LeftPage_KeyUp(object sender, KeyEventArgs e)
		{
			
		}

		private void LeftPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CurrentPage--;
			int count = CurrentPage * imageOnPage < imageName.Count ? imageOnPage : imageName.Count;
			imageControl.Load(
				ImageSize,
				imageName.GetRange(CurrentPage * imageOnPage, count)
			);
		}

		private void RightPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
            CurrentPage++;
			int count = CurrentPage * imageOnPage < imageName.Count ? imageOnPage : imageName.Count;
			imageControl.Load(
				ImageSize,
				imageName.GetRange(CurrentPage * imageOnPage, count)
			);
		}


		private void TextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				try
				{
					int count = CurrentPage * imageOnPage < imageName.Count ? imageOnPage : imageName.Count;
					imageControl.Load(
						ImageSize, 
						imageName.GetRange(CurrentPage * imageOnPage, count)
					);
				}
				catch (IOException)
				{
					Console.WriteLine("Файл занят ");
				}
				CurrentPage = 0;
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
				CurrentPage = 0;
				imageName = dataBase.Load(CurrentPage * imageOnPage, imageOnPage, filter).ToList();
				int count = imageOnPage < imageName.Count ? imageOnPage : imageName.Count;
				imageControl.Load(ImageSize, imageName.GetRange(CurrentPage, count));
			}
			catch (IOException)
			{
				Console.WriteLine("Файл занят ");
			}
		}
	}
}
