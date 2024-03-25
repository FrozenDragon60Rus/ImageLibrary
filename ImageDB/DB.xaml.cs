using System;
using System.Collections.Generic;
using System.Collections;
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
        public List<Table.Image> TableList = [];
        public Table.Image currentItem = null;
        public readonly DataBase dataBase = new("ImageLibrary", "Image");
        public readonly string[] join = ["Tag", "Character", "Author"];
        private string buffer = string.Empty;

        public DB()
        {
            InitializeComponent();

            DataInit();

            new TagButton(this, "Tag").Fill();
        }

        private void DataInit()
        {            
            ImageData.ItemsSource = TableList;
            dataBase.Load(ref TableList, join);
            AddColumn();
            if (TableList.Count > 0)
                currentItem = TableList.First();
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

        #region button_event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataBase.State == System.Data.ConnectionState.Open) return;

            System.Windows.Forms.FolderBrowserDialog FBD = new();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataBase.FromFolder(ref TableList, FBD.SelectedPath, "Address", Table.Image.Empty.parameter);
                XML.Info.folder = FBD.SelectedPath;
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
            if (currentItem == null) return;
            TableList.Remove(currentItem);
            dataBase.Delete(currentItem);
            ImageData.Items.Refresh();
        }
        private void Button_Delete_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentItem == null) return;
            int selectRow = TableList.IndexOf(currentItem);

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
            {
                dataBase.Delete(TableList.GetRange(selectRow, TableList.Count - selectRow));
                TableList.RemoveRange(selectRow, TableList.Count - selectRow);
            }
            
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
                currentItem = ImageData.CurrentCell.Item as Table.Image;
        }

        

        private void ImageData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if(ImageData.CurrentCell.IsValid)
                currentItem = ImageData.CurrentCell.Item as Table.Image;
        }

        private void ImageData_KeyUp(object sender, KeyEventArgs e)
        {
            string marker = ImageData.CurrentCell.Column.Header.ToString();
            DataBase ImageBy = new("ImageLibrary", "ImageBy" + marker);
            if (!join.Contains(marker)) return;
            switch (e.Key)
            {
                case Key.Delete:
                    ImageBy.Delete(currentItem.Id);
                    currentItem.parameter[marker] = string.Empty;
                    break;
                case Key.Back:
                    if (currentItem.parameter[marker].ToString() == string.Empty)
                        return;
                    string _marker = currentItem.parameter[marker]
                                                .ToString()
                                                .Split(',')
                                                .Last();
                    int markerId = GetButtonId(TagsGroup.Children, _marker);
                    ImageBy.Delete(currentItem.Id, markerId);
                    
                    currentItem.parameter = dataBase.LoadById(currentItem.Id, join);
                    break;
                case Key.C:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                        buffer = currentItem.parameter[marker].ToString();
                    break;
                case Key.V:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        currentItem.parameter[marker] = buffer;
                        ImageBy
                    }
                    break;
                case Key.X:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        buffer = (currentItem.parameter[marker] as string);
                        currentItem.parameter[marker] = string.Empty;
                    }
                    break;
            }
            ImageData.Items.Refresh();
        }
        private int GetButtonId(UIElementCollection children, string _marker)
        {
            foreach (Button button in TagsGroup.Children)
                if (button.Content.ToString() == _marker)
                    return (int)button.Tag;
            return -1;
        }
    }
}
