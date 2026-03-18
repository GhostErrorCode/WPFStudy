using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Modules.ToDo.Resources.Converters
{
    // 转换器: 当前选中项转换
    public class ToDoStatusSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 搜索待办事项状态参数 -> ComboBox集合中的项
            // 如果是null就转成字符"All"（表示搜索所有状态的待办事项）
            if(value == null)
            {
                return "All";
            }
            // 如果是枚举ToDoStatusEnum项，就直接返回（表示搜索状态为待办或已完成的待办事项）
            if(value is TodoStatusEnum status)
            {
                return status;
            }
            // 保底
            return Binding.DoNothing;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ComboBox当前选中的项 -> 搜索待办事项状态的参数
            // 如果当前选中的字符"All"，就返回null，表明搜索全部状态的待办事项
            if(value is string str && str == "All")
            {
                return null;
            }
            // 如果当前选中的是枚举ToDoStatusEnum项，就直接返回给VM层的参数
            if(value is TodoStatusEnum status)
            {
                return status;
            }
            // 保底
            return Binding.DoNothing;
        }
    }
}
