using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using NetworkModel;

namespace SampleCode
{
    [ValueConversion(typeof(NetworkType), typeof(Visibility))]
    public class NetworkTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = value as NetworkType?;
            var comboType = parameter as string;

            return (comboType == "Connection" && type == NetworkType.Mealy)
                   || (comboType == "Node" && type == NetworkType.Moore)
                ? Visibility.Visible
				: Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
