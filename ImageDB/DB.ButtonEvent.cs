using System;
using System.Windows;
using System.Windows.Input;
using ImageDB.SQL;

namespace ImageDB
{
    public partial class DB : Window
    {
        #region button_event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataBase.State == System.Data.ConnectionState.Open) return;

            System.Windows.Forms.FolderBrowserDialog FBD = new();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataBase.FromFolder(ref TableList, FBD.SelectedPath, "Address");
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

            int index = TableList.IndexOf(CurrentItem);
            TableList[index] = new(
                dataBase.LoadById(CurrentItem.Id, join));
            CurrentItem = TableList[index];

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
    }
}
