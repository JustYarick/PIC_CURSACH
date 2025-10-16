using System.Globalization;
using System.Windows.Data;

namespace PIC_CURSACH.utils;

public class NullToBoolConverter : IValueConverter
{
    public bool Invert { get; set; } = false;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNull = value == null;
        return Invert ? isNull : !isNull;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}