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
        private int selectRow = -1;
        public DB()
        {
            InitializeComponent();

            //Array.Sort(SeatchPatterns.Tag, StringComparer.InvariantCulture);
            new TagButton(this, "Tag").Fill();

            Init();
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
            foreach(string name in TableList.First().parameter.Keys)
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

        private ListBox CustomListBox()
        {
            ListBox l = new ListBox()
            {
                Width = 100,
                Height = 100,
                Margin = new Thickness(0, 0, 0, 0)
            };
            return l;
        }

        public void FindImage(string pattern = null)
        {

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
            }
            ImageData.Items.Refresh();
            Console.WriteLine(ImageData.Items.Count);
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
            //Console.WriteLine(currentItem.Name);
            TableList.Remove(currentItem);
            dataBase.Delete(currentItem);
            ImageData.Items.Refresh();
        }
        private void Button_Delete_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentItem == null) return;
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                TableList.RemoveRange(selectRow, TableList.Count - selectRow);
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
            selectRow = TableList.IndexOf(ImageData.CurrentCell.Item as Table.Image);
            if (selectRow > -1) currentItem = TableList[selectRow];
        }

        

        private void ImageData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if(ImageData.CurrentCell.IsValid)
                currentItem = ImageData.CurrentCell.Item as Table.Image;
        }

        private void ImageData_KeyUp(object sender, KeyEventArgs e)
        {
            string marker = "Tag";
            if (ImageData.CurrentCell.Column.Header.ToString() != marker) return;
            switch (e.Key)
            {
                case Key.Delete:
                    TableList[selectRow].parameter[marker] = string.Empty;
                    ImageData.Items.Refresh();
                    break;
                case Key.Back:
                    int pos = (TableList[selectRow].parameter[marker] as string).LastIndexOf(";");
                    if (pos < 0)
                    {
                        TableList[selectRow].parameter[marker] = string.Empty;
                        break;
                    }
                    TableList[selectRow].parameter[marker] = (TableList[selectRow].parameter[marker] as string).Remove(pos, (TableList[selectRow].parameter[marker] as string).Length - pos);
                    ImageData.Items.Refresh();
                    break;
                case Key.C:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        buffer = TableList[selectRow].parameter[marker].ToString();
                    }
                    break;
                case Key.V:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        TableList[selectRow].parameter[marker] = buffer;
                        ImageData.Items.Refresh();
                    }
                    break;
                case Key.X:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        buffer = (TableList[selectRow].parameter[marker] as string);
                        TableList[selectRow].parameter[marker] = string.Empty;
                        ImageData.Items.Refresh();
                    }
                    break;
            }
        }
    }
}
