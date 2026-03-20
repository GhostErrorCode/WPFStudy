using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Modules.ToDo.Resources.Converters
{
    // 转换器: 集合Count -> 可见性
    public class ToDoItemsCountToVisibility : IValueConverter
    {
        // VM -> View
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果传入的集合Count值是int且大于0，就不显示
            if (value is int count && count > 0)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        // View -> VM
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
