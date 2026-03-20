using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Media;

namespace Mosico.Utils;

[ValueConversion(typeof(SolidColorBrush), typeof(Color))]
public class BrushToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        ((SolidColorBrush)value).Color;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        new SolidColorBrush((Color)value);
}

public class FriendlyEnumConverter(Type type) : EnumConverter(type)
{
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            return value == null ? string.Empty :
                Regex.Replace(
                        value.ToString() ?? "",
                        "(?<=[^A-Z])([A-Z])", " $1",
                        RegexOptions.Compiled
                    ).Trim();
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
