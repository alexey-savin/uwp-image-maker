using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace SSD.MakeImagesForStore
{
    class ViewModel : INotifyPropertyChanged
    {
        private StorageFile _selectedFile = null;
        private StorageFolder _targetFolder = null;
        private BitmapImage _image = null;
        private string _targetFilenameTemplate = string.Empty;
        private List<BitmapSize> _targetSizes = new List<BitmapSize>();

        public ICommand MakeImagesCommand { get; set; }
        public ICommand BrowseSourceImageCommand { get; set; }
        public ICommand BrowseTargetCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            TargetFilenameTemplate = "output_";

            MakeImagesCommand = new CommandMake(
                p => MakeImages(), 
                p => CanMakeImages());
            BrowseSourceImageCommand = new CommandBrowseSourceImage(
                p => BrowseSourceImage());
            BrowseTargetCommand = new CommandBrowseTarget(
                p => BrowseTarget());

            AddTargetSquareSize(310);
            AddTargetSquareSize(150);
            AddTargetSquareSize(200);
            AddTargetSquareSize(100);
            AddTargetSquareSize(75);
            AddTargetSquareSize(63);
            AddTargetSquareSize(50);
            AddTargetSquareSize(176);
            AddTargetSquareSize(88);
            AddTargetSquareSize(44);
            AddTargetSquareSize(66);
            AddTargetSquareSize(55);
            AddTargetSquareSize(256);
            AddTargetSquareSize(48);
            AddTargetSquareSize(99);
            AddTargetSquareSize(210);
            AddTargetSquareSize(62);
            AddTargetSquareSize(70);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string TargetFilenameTemplate
        {
            get { return _targetFilenameTemplate; }
            set
            {
                _targetFilenameTemplate = value;
                OnPropertyChanged(nameof(TargetFilenameTemplate));
            }
        }

        public StorageFolder TargetFolder
        {
            get { return _targetFolder; }
            set
            {
                if (_targetFolder == null || !_targetFolder.Equals(value.Path))
                {
                    _targetFolder = value;

                    OnPropertyChanged(nameof(TargetFolder));
                    CanMakeImagesChanged();
                }
            }
        }

        public StorageFile SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (_selectedFile == null || !_selectedFile.Name.Equals(value.Name))
                {
                    _selectedFile = value;
                    CreateBitmapImage();

                    OnPropertyChanged(nameof(SelectedFile));
                    CanMakeImagesChanged();
                }
            }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public List<BitmapSize> TargetSizes
        {
            get { return _targetSizes; }
        }

        private async void CreateBitmapImage()
        {
            if (_selectedFile != null)
            {
                var stream = await _selectedFile.OpenReadAsync();
                var image = new BitmapImage();
                await image.SetSourceAsync(stream);

                Image = image;
            }
        }

        private void AddTargetSquareSize(uint size)
        {
            _targetSizes.Add(new BitmapSize() { Height = size, Width = size });
        }

        public bool CanMakeImages() => _selectedFile != null && _targetFolder != null;

        private async void MakeImages()
        {
            using (var sourceStream = await _selectedFile.OpenReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(sourceStream);

                using (var targetStream = new InMemoryRandomAccessStream())
                {
                    foreach (var targetSize in _targetSizes)
                    {
                        targetStream.Size = 0;

                        var encoder = await BitmapEncoder.CreateForTranscodingAsync(targetStream, decoder);
                        encoder.BitmapTransform.ScaledHeight = targetSize.Height;
                        encoder.BitmapTransform.ScaledWidth = targetSize.Width;
                        await encoder.FlushAsync();

                        var targetFilename = $"{_targetFilenameTemplate}{targetSize.Width}x{targetSize.Height}";
                        var targetFile = await _targetFolder.CreateFileAsync($"{targetFilename}.png", CreationCollisionOption.ReplaceExisting);

                        using (var targetFileStream = await targetFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await RandomAccessStream.CopyAndCloseAsync(targetStream.GetInputStreamAt(0), targetFileStream.GetOutputStreamAt(0));
                        }
                    }
                }
            }
        }

        private void CanMakeImagesChanged()
        {
            (MakeImagesCommand as CommandMake)?.RaiseCanExecuteChanged();
        }

        private async void BrowseSourceImage()
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
                SelectedFile = file;
            }
        }

        private async void BrowseTarget()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.ViewMode = PickerViewMode.Thumbnail;
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                TargetFolder = folder;
            }
        }
    }
}
