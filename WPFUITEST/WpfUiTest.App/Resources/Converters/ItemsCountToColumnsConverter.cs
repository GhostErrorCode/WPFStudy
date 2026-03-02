using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace WpfUiTest.App.Resources.Converters
{
    // 转换器: ItemsControl的数量 -> UniformGrid的列数
    public class ItemsCountToColumnsConverter : IValueConverter
    {
        // 正向转换：集合数量 → 列数
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果是集合且数量>0，列数=数量；否则默认1列
            if (value is int num && num > 0)
            {
                return num;
            }
            return 1; // 无数据时默认1列
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
