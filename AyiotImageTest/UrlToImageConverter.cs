using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
namespace AyiotImageTest
{
    public class UrlToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return null;
                ImageInfo image = value as ImageInfo;
                if (string.IsNullOrWhiteSpace(image.File)) return null;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnDemand;
                bi.UriSource = new Uri(image.File);
                bi.EndInit();
                image.ImgSource = bi;
                bi.DownloadCompleted += (sender, e) =>
                {

                };
                return bi.Clone();
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
