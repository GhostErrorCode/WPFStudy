using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.App.Resources.Converters
{
    // 转换器: 主题 -> 字符串
    public class ThemeToStringConverter : IValueConverter
    {
        /// <summary>
        /// ViewModel -> View
        /// </summary>
        /// <param name="value">原值，待转换的值</param>
        /// <param name="targetType">目标值，转换后的值</param>
        /// <param name="parameter">自定义可选参数（手动传值，不用则为null）</param>
        /// <param name="culture">区域/文化信息（多语言、格式化时用）</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 判断原值类型是否为Theme枚举类型并赋值给theme
            if (value is Theme theme)
            {
                /*
                // 普通写法，新手友好
                string themeName;
                switch (theme)
                {
                    case Theme.Light: themeName = "明亮"; break;
                    case Theme.Dark: themeName = "暗黑"; break;
                    case Theme.HighContrast: themeName = "高对比度"; break;
                    default: themeName = theme.ToString(); break;
                }
                return themeName;
                */

                // Switch表达式写法
                return theme switch
                {
                    Theme.Light => "明亮",
                    Theme.Dark => "暗黑",
                    Theme.HighContrast => "高对比度",
                    _ => theme.ToString()  // 兜底
                };
            }

            // Binding.DoNothing 是 System.Windows.Data.Binding 类的静态只读常量，属于 WPF 绑定系统的专属标识
            // 本次转换没有有效结果，不要把任何值传递给绑定的目标 UI 元素，也不要改变目标元素的当前值
            return Binding.DoNothing;
        }

        // View -> ViewModel
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
