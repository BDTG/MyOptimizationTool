﻿// In folder: Converters/BooleanToVisibilityConverter.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MyOptimizationTool.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}