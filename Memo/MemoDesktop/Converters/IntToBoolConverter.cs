using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MemoDesktop.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        /// <summary>
        /// 正向转换方法：将源数据转换为UI显示所需的值
        /// </summary>
        /// <param name="value">绑定源的值，通常是整数，表示数量</param>
        /// <param name="targetType">目标类型，这里应该是Visibility类型</param>
        /// <param name="parameter">转换参数，可控制反转逻辑（如"inverse"）</param>
        /// <param name="culture">区域文化信息</param>
        /// <returns>Visibility：如果数量为0则返回Visible，否则Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null && int.TryParse(value.ToString(), out int result))
            {
                if (result == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 反向转换方法：将UI的值转换回源数据格式
        /// </summary>
        /// <param name="value">Visibility值</param>
        /// <param name="targetType">目标类型，整数类型</param>
        /// <param name="parameter">转换参数</param>
        /// <param name="culture">区域文化信息</param>
        /// <returns>通常返回DependencyProperty.UnsetValue，表示不进行反向转换</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 这里是前端传给后端（bool 转 int）
            if (value != null && bool.TryParse(value.ToString(), out bool result))
            {
                if (result == true)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
