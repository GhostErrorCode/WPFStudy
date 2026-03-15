using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace WpfUiTest.Controls.Resources.Converters
{
    // ComboBox项 页大小 转 字符说明
    public class PageSizeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果传入的是数值整形，就转为xxx条/页
            if(value is int pageSize)
            {
                return $"{pageSize}条/页";
            }
            // 保底：返回原样
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
