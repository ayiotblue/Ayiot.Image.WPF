using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AyiotImageTest
{
    public class ImageInfo : INotifyPropertyChanged
    {
        public string File { get; set; }
        public string Name { get; set; }
        public ImageSource _imgSource;
        public ImageSource ImgSource
        {
            get { return _imgSource; }
            set
            {
                if (_imgSource != value)
                {
                    _imgSource = value;
                    NotifyPropertyChanged("ImgSource");
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
