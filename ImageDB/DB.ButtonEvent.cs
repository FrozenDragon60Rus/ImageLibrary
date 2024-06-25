using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
				dataBase.Clear(join);

				var files = Directory.GetFiles(FBD.SelectedPath);
                int index = 1;
				foreach (var file in files)
					dataBase.Add(new Table.Image(index++, file, 0));

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
			var address = dataBase.Image.Select(t => t.Parameter["Address"]);
            files = files.Except(address);

            int index = dataBase.Image.Count + 1;
			foreach (var file in files)
				dataBase.Add(new Table.Image(index++, file.ToString(), 0));

			ImageData.Items.Refresh();
        }
        private void Button_Delete(object sender, RoutedEventArgs e)
        {
			if(dataBase.State == System.Data.ConnectionState.Open) return;

			if (CurrentItem == null) return;

            dataBase.Delete(CurrentItem);
            ImageData.Items.Refresh();
        }
        private void Button_Delete_KeyDown(object sender, KeyEventArgs e)
        {
			if (dataBase.State == System.Data.ConnectionState.Open) return;
			if (CurrentItem == null) return;
            int selectRow = dataBase.Image.IndexOf(CurrentItem);

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                dataBase.Delete(
                    dataBase.Image.GetRange(
                        selectRow, 
                        dataBase.Image.Count - selectRow));

            ImageData.Items.Refresh();
        }
        public void AddEvent(object sender, EventArgs e)
        {
			if (dataBase.State == System.Data.ConnectionState.Open) return;

			var button = sender as MarkerButton;
            var marker = new MarkerDataBase("ImageLibrary", "ImageBy" + button.Marker);

            marker.Add(CurrentItem.Id, button.Id, button.Marker);

			CurrentItem = dataBase.Update(CurrentItem);

            ImageData.Items.Refresh();
        }
        public void RemoveEvent(object sender, EventArgs e)
        {
			if (dataBase.State == System.Data.ConnectionState.Open) return;

			var button = sender as MarkerButton;
            var marker = new MarkerDataBase("ImageLibrary", "ImageBy" + button.Marker);

            marker.Delete(CurrentItem.Id, button.Id);

            ImageData.Items.Refresh();
        }
        #endregion
    }
}
