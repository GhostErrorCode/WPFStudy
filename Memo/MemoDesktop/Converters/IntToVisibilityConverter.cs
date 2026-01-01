using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MemoDesktop.Converters
{
    public class IntToVisibilityConverter : IValueConverter
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
            
            if (value != null && int.TryParse(value.ToString(),out int result))
            {
                if(result == 0)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Hidden;
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
            throw new NotImplementedException();
        }
    }
}

/*
Convert方法参数：

    value - 从ViewModel传来的原始数据

    targetType - 要转换成的目标类型（由绑定系统自动确定）

    parameter - 在XAML中传递的额外参数，用于控制转换逻辑

    culture - 文化信息，用于本地化处理（通常用不上）

ConvertBack方法参数：

    value - 从UI传来的当前值

    targetType - 要转换回的数据源类型

    parameter - 同上

    culture - 同上
*/