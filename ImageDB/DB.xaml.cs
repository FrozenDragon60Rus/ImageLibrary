using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageDB.SQL;
using System.Windows.Data;
using System.Runtime.Versioning;

namespace ImageDB
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    [SupportedOSPlatform("Windows")]
    public partial class DB : Window
    {
        private List<Table.Image> TableList = [];
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
            dataBase.Load(ref TableList, join);
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
                    IsReadOnly = true
                };
                if (join.Contains(column.Header)) column.MaxWidth = 150;

                ImageData.Columns.Add(column);
            }
        }
        private void FillTagButton()
        {
            DataBase tag = new("ImageLibrary", join.First());
            List<Table.Marker> marker = [];

            tag.Load(ref marker);

            foreach (Table.Marker Tag in marker)
                TagsGroup.Children.Add(new MarkerButton(Tag.Name, Tag.Id, join.First(), AddEvent, RemoveEvent));
        }

        #region button_event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataBase.State == System.Data.ConnectionState.Open) return;

            System.Windows.Forms.FolderBrowserDialog FBD = new();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataBase.FromFolder(ref TableList, FBD.SelectedPath, "Address", Table.Image.Empty.parameter);
                XML.Info.Folder = FBD.SelectedPath;
                dataBase.Load(ref TableList, join);
            }
            ImageData.Items.Refresh();
        }

        private void Button_Refresh(object sender, RoutedEventArgs e)
        {
            if (dataBase.State == System.Data.ConnectionState.Open) return;
            dataBase.Refresh(ref TableList, "Address");
            ImageData.Items.Refresh();
            
        }
        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            if (CurrentItem == null) return;
            TableList.Remove(CurrentItem);
            dataBase.Delete(CurrentItem);
            ImageData.Items.Refresh();
        }
        private void Button_Delete_KeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentItem == null) return;
            int selectRow = TableList.IndexOf(CurrentItem);

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
            {
                dataBase.Delete(TableList.GetRange(selectRow, TableList.Count - selectRow));
                TableList.RemoveRange(selectRow, TableList.Count - selectRow);
            }
            
            ImageData.Items.Refresh();
        }
        public void AddEvent(object sender, EventArgs e)
        {
            var button = sender as MarkerButton;
            DataBase marker = new("ImageLibrary", "ImageBy" + button.Marker);

            marker.Add(CurrentItem.Id, button.Id, button.Marker);
            CurrentItem.parameter = dataBase.LoadById(CurrentItem.Id, join);

            ImageData.Items.Refresh();
        }
        public void RemoveEvent(object sender, EventArgs e)
        {
            var button = sender as MarkerButton;

            DataBase marker = new("ImageLibrary", "ImageBy" + button.Marker);

            marker.Delete(CurrentItem.Id, button.Id);

            ImageData.Items.Refresh();
        }
        #endregion

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource;

            if (itemsSource is null)  
                yield return null;

            foreach (var item in itemsSource)
                if (grid.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row) 
                    yield return row;
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
                    CurrentItem.parameter[marker] = string.Empty;
                    break;
                case Key.Back:
                    if (CurrentItem.parameter[marker]
                                   .ToString() == string.Empty)
                        return;
                    string name = CurrentItem.parameter[marker]
                                                .ToString()
                                                .Split(',')
                                                .Last();
                    int markerId = GetButtonId(name);
                    ImageBy.Delete(CurrentItem.Id, markerId);

                    CurrentItem.parameter = dataBase.LoadById(CurrentItem.Id, join);
                    break;
                case Key.C:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        buffer = CurrentItem.parameter[marker].ToString();
                    break;
                case Key.V:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        CurrentItem.parameter[marker] = buffer;

                    }
                    break;
                case Key.X:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        buffer = (CurrentItem.parameter[marker] as string);
                        CurrentItem.parameter[marker] = string.Empty;
                    }
                    break;
            }
            ImageData.Items.Refresh();
        }
        private int GetButtonId(string name)
        {
            foreach (Button button in TagsGroup.Children)
                if (button.Content.ToString() == name)
                    return (int)button.Tag;
            return -1;
        }
    }
}
