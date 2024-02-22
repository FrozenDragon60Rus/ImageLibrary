using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ImageDB.SQL;
using System.Runtime.Remoting.Messaging;
using System.Windows.Data;

namespace ImageDB
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DB : Window
    {
        public static List<Table.Image> TableList = new List<Table.Image>();
        public Table.Image currentItem = null;
        public readonly DataBase dataBase = new DataBase("ImageLibrary", "Image");
        public readonly string[] join = new string[] { "Tag", "Character", "Author" };
        string buffer = string.Empty;
        public DB()
        {
            InitializeComponent();

            Init();
            //Array.Sort(SeatchPatterns.Tag, StringComparer.InvariantCulture);
            new TagButton(this, "Tag").Fill();
        }

        public void Init()
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

                ImageData.Columns.Add(column);
            }
        }

        #region button_event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataBase.State == System.Data.ConnectionState.Open) return;

            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
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
            selectRow = -1;
        }
        private void Button_Document_List(object sender, RoutedEventArgs e)
        {
            //DBControl.FormDocumentList();
        }
        #endregion

            public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            IEnumerable itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (IEnumerable item in itemsSource)
            {
                DataGridRow row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
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
            DataBase ImageBy = new DataBase("ImageLibrary", "ImageBy" + marker);
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
                    {
                        buffer = currentItem.parameter[marker].ToString();
                    }
                    break;
                case Key.V:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        currentItem.parameter[marker] = buffer;
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
