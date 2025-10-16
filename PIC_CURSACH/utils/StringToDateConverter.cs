using System.Globalization;
using System.Windows.Data;

namespace PIC_CURSACH.utils;

public class StringToDateConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string dateString && DateTime.TryParse(dateString, out var date))
        {
            return date;
        }
        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        return string.Empty;
    }
}