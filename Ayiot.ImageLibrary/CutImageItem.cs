using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ayiot.ImageLibrary
{
    public class CutImageItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public Size CutSize { get; set; }
        bool _isComplete;
        public bool IsComplete
        {
            get
            {
                return _isComplete;
            }
            set
            {
                _isComplete = value;
                RaisedChanged("IsComplete");
            }
        }
        private bool _isChange = false;
        public bool IsChange { get { return this._isChange; } }

        private ImageSource imgSource = null;
        public ImageSource ImgSource
        {
            get
            {
                return imgSource;
            }
            set
            {
                imgSource = value;
                RaisedChanged("ImgSource");
            }
        }

        public void CreateImageSource(string filename)
        {
            try
            {
                if (ImgSource == null)
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnDemand;
                    bi.UriSource = new Uri(filename);
                    bi.EndInit();
                    ImgSource = bi;
                }
            }
            catch
            {
                ImgSource = null;
            }
        }

        /// <summary>
        /// 切图的宽高比
        /// </summary>
        public double Scale
        {
            get
            {
                return CutSize.Width / CutSize.Height;
            }
        }

        public string StrSize
        {
            get
            {
                return CutSize.Width.ToString().PadLeft(2, '0') + CutSize.Height.ToString().PadLeft(2, '0');
            }
        }

        private void RaisedChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                this._isChange = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Id.ToString();
        }

    }
}
