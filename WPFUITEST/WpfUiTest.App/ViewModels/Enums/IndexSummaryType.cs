using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.App.ViewModels.Enums
{
    // 首页汇总枚举类
    // 用于枚举每项的作用
    public enum IndexSummaryType
    {
        // 待办总量
        ToDoTotal,
        // 已完成
        ToDoCompleted,
        // 完成率
        ToDoCompletionRate,
        // 备忘录总量
        MemoTotal,
        // 自定义汇总
        Custom
    }
}
