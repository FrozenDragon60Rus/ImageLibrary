using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml.Linq;
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
				dataBase.Clear(join);
				TableList.Clear();
				var files = Directory.GetFiles(FBD.SelectedPath);
                int index = 1;
				foreach (var file in files)
				{
					TableList.Add(new Table.Image(index++, file, 0));
					dataBase.Add(TableList.Last());
				}
				XML.Info.Folder = FBD.SelectedPath;
			}
			ImageData.Items.Refresh();

		}

        private void Button_Refresh(object sender, RoutedEventArgs e)
        {
            if (dataBase.State == System.Data.ConnectionState.Open) return;

			if (XML.Info.Folder == string.Empty)
			{
				MessageBox.Show("База не была сформирована или отсутствуют данные о её формировании");
				return;
			}

			IEnumerable<object> files = Directory.GetFiles(XML.Info.Folder);

			string[] extensionList = ["jpg, png, jpeg, gif, bmp"];
			var address = TableList.Select(t => t.Parameter["Address"]);
            files = files.Except(address);

            int index = TableList.Count + 1;
			foreach (var file in files)
			{
				TableList.Add(new Table.Image(index++, file.ToString(), 0));
				dataBase.Add(TableList.Last());
			}
			ImageData.Items.Refresh();

        }
        private void Button_Delete(object sender, RoutedEventArgs e)
        {
			if(dataBase.State == System.Data.ConnectionState.Open) return;
			if (CurrentItem == null) return;
            TableList.Remove(CurrentItem);
            dataBase.Delete(CurrentItem);
            ImageData.Items.Refresh();
        }
        private void Button_Delete_KeyDown(object sender, KeyEventArgs e)
        {
			if (dataBase.State == System.Data.ConnectionState.Open) return;
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
			if (dataBase.State == System.Data.ConnectionState.Open) return;
			var button = sender as MarkerButton;
            DataBase marker = new("ImageLibrary", "ImageBy" + button.Marker);

            marker.Add(CurrentItem.Id, button.Id, button.Marker);

            int index = TableList.IndexOf(CurrentItem);
            if(index < 0) index = 0;

            TableList[index] = new(
                dataBase.LoadById(CurrentItem.Id, join));
            CurrentItem = TableList[index];

            ImageData.Items.Refresh();
        }
        public void RemoveEvent(object sender, EventArgs e)
        {
			if (dataBase.State == System.Data.ConnectionState.Open) return;

			var button = sender as MarkerButton;

            DataBase marker = new("ImageLibrary", "ImageBy" + button.Marker);

            marker.Delete(CurrentItem.Id, button.Id);

            ImageData.Items.Refresh();
        }
        #endregion
    }
}
