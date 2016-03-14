using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
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

        public ViewModel()
        {
            TargetFilenameTemplate = "output_";

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

        public event PropertyChangedEventHandler PropertyChanged;

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
                OnPropertyChanged("TargetFilenameTemplate");
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

                    OnPropertyChanged("TargetFolder");
                    OnPropertyChanged("CanMakeImages");
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

                    OnPropertyChanged("SelectedFile");
                    OnPropertyChanged("CanMakeImages");
                }
            }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }

        public List<BitmapSize> TargetSizes
        {
            get { return _targetSizes; }
        }

        public bool CanMakeImages
        {
            get
            {
                return (_selectedFile != null && _targetFolder != null);
            }
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
    }
}
