using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MinimalisticWPF
{
    public class XThicknessConverter : IValueConverter
    {
        public double LeftRate { get; set; } = 1;

        public double RightRate { get; set; } = 1;

        public double TopRate { get; set; } = 1;

        public double BottomRate { get; set; } = 1;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness temp = (Thickness)value;
            return new Thickness(temp.Left * LeftRate, temp.Top * TopRate, temp.Right * RightRate, temp.Bottom * BottomRate);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness temp = (Thickness)value;
            return new Thickness(temp.Left / LeftRate, temp.Top / TopRate, temp.Right / RightRate, temp.Bottom / BottomRate);
        }
    }
}
