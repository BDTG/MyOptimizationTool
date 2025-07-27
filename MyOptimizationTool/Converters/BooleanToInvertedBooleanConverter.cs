// In folder: Converters/BooleanToInvertedBooleanConverter.cs
using Microsoft.UI.Xaml.Data;
using System;

namespace MyOptimizationTool.Converters
{
    public class BooleanToInvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Trả về giá trị ngược lại của boolean đầu vào
            return !(value is bool b && b);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Logic tương tự cho TwoWay binding
            return !(value is bool b && b);
        }
    }
}