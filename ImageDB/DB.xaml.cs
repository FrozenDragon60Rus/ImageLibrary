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
        DataBase dataBase = new DataBase("ImageLibrary", "Image");
        string buffer = string.Empty;
        string[] join = new string[] { "Tag", "Character", "Author" };
        private int num = -1;
        public DB()
        {
            InitializeComponent();

            //Array.Sort(SeatchPatterns.Tag, StringComparer.InvariantCulture);
            FillTagPanel();

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

        private Button TagButton(string name, int num)
        {
            Button b = new Button()
            {
                Name = name + "_button",
                Content = name,
                Background = Brushes.White,
                FontSize = 16f,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 250,
                Height = 30,
                Margin = new Thickness(5,
                                       (5 + Height) * num,
                                       0,
                                       0),
            };
            b.Click += AddTagEvent;
            b.MouseRightButtonUp += RemoveTagEvent;

            return b;
        }
        private void FillTagPanel()
        {
            DataBase tag = new DataBase("ImageLibrary", "Tag");
            List<Table.Marker> marker = new List<Table.Marker>();
            
            tag.Load(ref marker);

            for (int i = 0; i < marker.Count; i++)
                TagsGroup.Children.Add(TagButton(marker[i].Name, i));
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

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            if (dataBase.State == System.Data.ConnectionState.Open) return;
            dataBase.Update(TableList);
        }

        private void Button_Refresh(object sender, RoutedEventArgs e)
        {
            if (dataBase.State == System.Data.ConnectionState.Open) return;
            dataBase.Refresh(ref TableList, "Address");
            ImageData.Items.Refresh();
            
        }
        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            if (num == -1) return;
            //Console.WriteLine(currentItem.Name);
            TableList.Remove(currentItem);
            ImageData.Items.Refresh();
        }
        private void Button_Delete_KeyDown(object sender, KeyEventArgs e)
        {
            if (num == -1) return;
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                TableList.RemoveRange(num, TableList.Count - num);
            ImageData.Items.Refresh();
            num = -1;
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
            num = TableList.IndexOf(ImageData.CurrentCell.Item as Table.Image);
            if (num > -1) currentItem = TableList[num];
        }

        public void AddTagEvent(object sender, EventArgs e)
        {
            Button TagButton = sender as Button;
            if (num == -1 || (TableList[num].parameter["Tag"] as string).Split(';')
                                                                        .Contains(TagButton.Content.ToString())) 
                return;
            TableList[num].parameter["Tag"] = (TableList[num].parameter["Tag"] as string) == string.Empty ? TableList[num].parameter["Tag"] + TagButton.Content.ToString()
                                                                                                          : TableList[num].parameter["Tag"] + ";" + TagButton.Content.ToString();

            ImageData.Items.Refresh();
        }
        public void RemoveTagEvent(object sender, EventArgs e)
        {
            Button TagButton = sender as Button;
            if (num == -1) return;
            if ((TableList[num].parameter["Tag"] as string)
                               .Split(';')
                               .Contains(TagButton.Content.ToString()))
            {
                string item = TagButton.Content as string;

                int pos = (TableList[num].parameter["Tag"] as string).IndexOf(item);
                int sim = (TableList[num].parameter["Tag"] as string).Length > item.Length ? 1 : 0;
                if (pos > 0) TableList[num].parameter["Tag"] = (TableList[num].parameter["Tag"] as string).Remove(pos - 1, item.Length + 1);
                else if (pos == 0) TableList[num].parameter["Tag"] = (TableList[num].parameter["Tag"] as string).Remove(pos, item.Length + sim);

                ImageData.Items.Refresh();
            }
        }

        private void ImageData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            currentItem = ImageData.CurrentCell.Item as Table.Image;
        }

        private void ImageData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ImageData_KeyUp(object sender, KeyEventArgs e)
        {
            string marker = "Tag";
            if (ImageData.CurrentCell.Column.Header.ToString() != marker) return;
            switch (e.Key)
            {
                case Key.Delete:
                    TableList[num].parameter[marker] = string.Empty;
                    ImageData.Items.Refresh();
                    break;
                case Key.Back:
                    int pos = (TableList[num].parameter[marker] as string).LastIndexOf(";");
                    if (pos < 0)
                    {
                        TableList[num].parameter[marker] = string.Empty;
                        break;
                    }
                    TableList[num].parameter[marker] = (TableList[num].parameter[marker] as string).Remove(pos, (TableList[num].parameter[marker] as string).Length - pos);
                    ImageData.Items.Refresh();
                    break;
                case Key.C:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        buffer = TableList[num].parameter[marker].ToString();
                    }
                    break;
                case Key.V:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        TableList[num].parameter[marker] = buffer;
                        ImageData.Items.Refresh();
                    }
                    break;
                case Key.X:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        buffer = (TableList[num].parameter[marker] as string);
                        TableList[num].parameter[marker] = string.Empty;
                        ImageData.Items.Refresh();
                    }
                    break;
            }
        }
    }
}
