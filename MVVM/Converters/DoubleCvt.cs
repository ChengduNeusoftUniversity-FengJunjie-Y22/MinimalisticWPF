using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MinimalisticWPF
{
    public class DoubleCvt : IValueConverter
    {
        public DoubleCvt() { }

        public double ConvertRate { get; set; } = 1;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * ConvertRate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / ConvertRate;
        }
    }
}
