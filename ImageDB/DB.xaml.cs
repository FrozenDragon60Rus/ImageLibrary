using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageDB.SQL;
using System.Windows.Data;
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
        private List<Table.Image> TableList { get; } = [];
        private Table.Image CurrentItem { set; get; } = null;
        public DataBase dataBase = new("ImageLibrary", "Image");
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
            ImageData.ItemsSource = TableList;
            TableList.AddRange(dataBase.Load<Table.Image>(join));
            Debug.WriteLine("LOAD: " + TableList.Count);
			AddColumn();
            if (TableList.Count > 0)
                CurrentItem = TableList.First();
            ImageData.Items.Refresh();
        }
        private void AddColumn()
        {
            foreach(string name in dataBase.Columns.Concat(join))
            {
                var column = new DataGridTextColumn()
                {
                    Header = name,
                    Binding = new Binding(name),
                    IsReadOnly = true,
                };
                if (join.Contains(column.Header)) column.MaxWidth = 150;

                ImageData.Columns.Add(column);
            }
        }
        private void FillTagButton()
        {
            DataBase tag = new("ImageLibrary", join.First());
            List<Table.Marker> marker = tag.Load<Table.Marker>().ToList();

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
            var ImageBy = new DataBase("ImageLibrary", "ImageBy" + marker);

            if (!join.Contains(marker)) return;

            switch (e.Key)
            {
                case Key.Delete:
                    ImageBy.Delete(CurrentItem.Id);
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
                        dataBase.LoadById(CurrentItem.Id, join));
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
    }
}
