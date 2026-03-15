using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace WpfUiTest.Controls.Resources.Converters
{
    /// <summary>
    /// 将整数减 1 的转换器，用于上一页按钮的命令参数。
    /// 例如：绑定 CurrentPage，转换后得到 CurrentPage - 1。
    /// </summary>
    public class SubtractOneConverter : IValueConverter
    {
        // 上一页转换器
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 将当前的页码值减一并返回
            if (value is int pageNumber && pageNumber >= 2)
            {
                return --pageNumber;
            }

            // 如果页码值已是1，则直接返回1
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
