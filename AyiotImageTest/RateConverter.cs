using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AyiotImageTest
{
    public class RateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return (double)16 / 9;
            string rate = (string)value;
            string[] vs = rate.Split(':');
            if (vs.Length != 2)
                return (double)16 / 9;
            double width = double.Parse(vs[0]);
            double height = double.Parse(vs[1]);
            return width / height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
