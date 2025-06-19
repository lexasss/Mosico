﻿using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Mosico.Definitions;

[ValueConversion(typeof(SolidColorBrush), typeof(Color))]
public class BrushToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        ((SolidColorBrush)value).Color;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        new SolidColorBrush((Color)value);
}