using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SSD.MakeImagesForStore
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void buttonBrowseSourceImage_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".png");

            StorageFile file = await filePicker.PickSingleFileAsync();
            // for multiple selection
            //IReadOnlyList<StorageFile> fileList = await filePicker.PickMultipleFilesAsync();
            if (file != null)
            {
                ViewModel.SelectedFile = file;
            }
        }

        private async void buttonBrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.ViewMode = PickerViewMode.Thumbnail;
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                ViewModel.TargetFolder = folder;
            }
        }

        private async void buttonMakeImages_Click(object sender, RoutedEventArgs e)
        {
            textBlockDone.Visibility = Visibility.Collapsed;

            using (var sourceStream = await ViewModel.SelectedFile.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(sourceStream);
                
                using (var targetStream = new InMemoryRandomAccessStream())
                {
                    foreach (var targetSize in ViewModel.TargetSizes)
                    {
                        targetStream.Size = 0;

                        var encoder = await BitmapEncoder.CreateForTranscodingAsync(targetStream, decoder);
                        encoder.BitmapTransform.ScaledHeight = targetSize.Height;
                        encoder.BitmapTransform.ScaledWidth = targetSize.Width;
                        await encoder.FlushAsync();

                        var targetFilename = $"{ViewModel.TargetFilenameTemplate}{targetSize.Width}x{targetSize.Height}";
                        var targetFile = await ViewModel.TargetFolder.CreateFileAsync($"{targetFilename}.png", CreationCollisionOption.ReplaceExisting);

                        using (var targetFileStream = await targetFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await RandomAccessStream.CopyAndCloseAsync(targetStream.GetInputStreamAt(0), targetFileStream.GetOutputStreamAt(0));
                        }
                    }
                }
            }

            textBlockDone.Visibility = Visibility.Visible;
        }

        private ViewModel ViewModel
        {
            get { return (ViewModel)DataContext; }
        }
    }
}
