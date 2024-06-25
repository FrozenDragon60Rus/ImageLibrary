using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageDB.SQL;
using System.Runtime.Versioning;
using System.Diagnostics;

namespace ImageDB
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    [SupportedOSPlatform("Windows")]
    public partial class DB : Window
    {
        //private List<Table.Image> TableList { get; } = [];
        private Table.Image CurrentItem { set; get; } = null;
        public ImageDataBase dataBase;
        public readonly string[] join = ["Tag", "Character", "Author"];
        private string buffer = string.Empty;

        public DB()
        {
            InitializeComponent();

            DataInit();

            FillTagButton();
        }

        private void DataInit()
        {
            dataBase = new("ImageLibrary", "Image", join);

			ImageData.ItemsSource = dataBase.Image;

            if (dataBase.Image.Count > 0)
                CurrentItem = dataBase.Image.First();

            ImageData.Items.Refresh();
        }
        private void FillTagButton()
        {
            MarkerDataBase tag = new("ImageLibrary", join.First());
            List<Table.Marker> marker = tag.Get<Table.Marker>().ToList();

            foreach (Table.Marker Tag in marker)
                TagsGroup.Children.Add(new MarkerButton(Tag.Name, Tag.Id, join.First(), AddEvent, RemoveEvent));
        }

        private void ImageData_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ImageData_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ImageData.CurrentCell.IsValid)
                CurrentItem = ImageData.CurrentCell.Item as Table.Image;
            
        }

        

        private void ImageData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
            if(ImageData.CurrentCell.IsValid)
                CurrentItem = ImageData.CurrentCell.Item as Table.Image;
        }

        private void ImageData_KeyUp(object sender, KeyEventArgs e)
        {
            string marker = ImageData.CurrentCell.Column.Header.ToString();
            var ImageBy = new MarkerDataBase("ImageLibrary", "ImageBy" + marker);

            if (!join.Contains(marker)) return;

            switch (e.Key)
            {
                case Key.Delete:
                    dataBase.Delete(CurrentItem.Id);
                    CurrentItem.Parameter[marker] = string.Empty;
                    break;
                case Key.Back:
                    if (CurrentItem.Parameter[marker]
                                   .ToString() == string.Empty)
                        return;

                    string name = CurrentItem.Parameter[marker]
                                                .ToString()
                                                .Split(',')
                                                .Last();

                    int markerId = GetButtonId(name);
                    ImageBy.Delete(CurrentItem.Id, markerId);

                    CurrentItem = new(
                        dataBase.Get<Table.Image>(CurrentItem.Id));
                    break;
                case Key.C:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        buffer = CurrentItem.Parameter[marker].ToString();
                    break;
                case Key.V:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        CurrentItem.Parameter[marker] = buffer;
                        string[] markerList = buffer.Split(",");
                        foreach (string item in markerList)
                            ImageBy.Add(CurrentItem.Id, GetButtonId(item), marker);
                    }
                    break;
                case Key.X:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        buffer = (CurrentItem.Parameter[marker] as string);
                        CurrentItem.Parameter[marker] = string.Empty;
                    }
                    break;
            }
            ImageData.Items.Refresh();
        }
        private int GetButtonId(string name)
        {
            foreach (MarkerButton button in TagsGroup.Children)
                if (button.Content.ToString() == name)
                    return button.Id;
            return -1;
        }

		private void ImageData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
            
		}
	}
}
