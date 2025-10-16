using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PIC_CURSACH.utils;

public class BoolToVisibilityConverter : IValueConverter
{
    // Singleton instance for easy XAML usage
    public static readonly BoolToVisibilityConverter Instance = new();

    // Converts bool to Visibility: true -> Visible, false -> Collapsed
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolVal)
        {
            return boolVal ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    // Converts Visibility back to bool
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility vis)
        {
            return vis == Visibility.Visible;
        }
        return false;
    }
}