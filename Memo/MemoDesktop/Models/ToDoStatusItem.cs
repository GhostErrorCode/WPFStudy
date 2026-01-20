using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoDesktop.Models
{
    public  class ToDoStatusItem
    {
        public string? DisplayName { get; set; } = string.Empty; // 待办事项状态显示名称
        public int? Value { get; set; }  // 待办事项状态具体值
    }
}
