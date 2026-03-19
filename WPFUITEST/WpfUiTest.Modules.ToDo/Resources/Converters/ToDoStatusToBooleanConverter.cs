using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Modules.ToDo.Resources.Converters
{
    // 待办状态 转 布尔值，用于菜单“完成此待办事项”是否可用
    public class ToDoStatusToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 根据传入的状态决定是否能可用
            // 待办 -> true，已完成 -> false
            if(value is TodoStatusEnum status)
            {
                return status == TodoStatusEnum.Pending ? true : false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
