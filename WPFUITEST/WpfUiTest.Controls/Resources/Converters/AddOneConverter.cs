using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace WpfUiTest.Controls.Resources.Converters
{
    /// <summary>
    /// 将整数加 1 的转换器，用于下一页按钮的命令参数。
    /// </summary>
    public class AddOneConverter : IValueConverter
    {
        // 下一页转换器
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 将当前的页码值加一并返回
            if(value is int pageNumber)
            {
                return ++pageNumber;
            }

            // 最终保底，直接返回1
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
