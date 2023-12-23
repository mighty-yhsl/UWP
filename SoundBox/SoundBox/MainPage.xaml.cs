using Windows.Storage;
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;



namespace SoundBox
{

    public sealed partial class MainPage : Page
    {
        private List<StorageFile> selectedFiles = new List<StorageFile>();
        private List<StorageFile> playlist = new List<StorageFile>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            folderPicker.FileTypeFilter.Add(".mp3");
            StorageFolder selectedFolder = await folderPicker.PickSingleFolderAsync();

            if (selectedFolder != null)
            {
                IReadOnlyList<StorageFile> files = await selectedFolder.GetFilesAsync();
                selectedFiles = files.ToList();
                FileListView.ItemsSource = selectedFiles.Select(file => file.Name);
            }
        }

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            playlist = selectedFiles.ToList();
        }

        private async void PlayPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = new MediaElement();

            foreach (StorageFile file in playlist)
            {
                var stream = await file.OpenAsync(FileAccessMode.Read);
                mediaElement.SetSource(stream, file.ContentType);
                mediaElement.Play();

                await Task.Delay(5000);
                mediaElement.Stop();
            }
        }

        private void SearchTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.ToLower();
            List<StorageFile> searchResults = selectedFiles.Where(file => file.Name.ToLower().Contains(searchTerm)).ToList();
            FileListView.ItemsSource = searchResults.Select(file => file.Name);
        }

        private void SortFilesByName()
        {
            selectedFiles = selectedFiles.OrderBy(file => file.Name).ToList();
            FileListView.ItemsSource = selectedFiles.Select(file => file.Name);
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            SortFilesByName();
        }
    }
}
