// In folder: Converters/BooleanToInvertedBooleanConverter.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MyOptimizationTool.Converters
{
    public class BooleanToInvertedBooleanConverter : IValueConverter
    {
        // Cú pháp đúng: public [kiểu trả về] [tên phương thức]
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool b && b);
        }

        // Cú pháp đúng: public [kiểu trả về] [tên phương thức]
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}