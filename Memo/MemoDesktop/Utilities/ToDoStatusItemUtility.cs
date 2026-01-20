using MemoDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MemoDesktop.Utilities
{
    public static class ToDoStatusItemUtility
    {
        // 初始化一个待办事项状态的列表
        public static ObservableCollection<ToDoStatusItem> GetToDoStatusItems()
        {
            return new ObservableCollection<ToDoStatusItem>()
            {
                new ToDoStatusItem(){ DisplayName = "全部", Value = null },
                new ToDoStatusItem(){ DisplayName = "待办", Value = 0 },
                new ToDoStatusItem(){ DisplayName = "已完成" ,Value = 1 }
            }; 
        }
    }
}
