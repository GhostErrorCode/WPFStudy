using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.App.Resources.Converters
{
    // 转换器: 主题 -> 字符串
    public class ToDoStatusToStringConverter : IValueConverter
    {
        // VM -> View
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 根据VM层的原值转为View要显示的值，枚举Pending 转为 字符待办
            if(value is TodoStatusEnum todoStatus)
            {
                return todoStatus switch
                {
                    TodoStatusEnum.Pending => "待办",
                    TodoStatusEnum.Completed => "已完成",
                    _ => "全部"  // 这里的全部是所有待办及已完成的
                };
            }
            // 未知异常，直接返回啥也没做
            return Binding.DoNothing;
        }

        // View -> VM
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 不需要View给VM返回值
            return Binding.DoNothing;
        }
    }
}
