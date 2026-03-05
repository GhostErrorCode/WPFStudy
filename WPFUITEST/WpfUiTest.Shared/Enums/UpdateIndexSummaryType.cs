using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Shared.Enums
{
    // 更新汇总数据类型，用于首页添加/修改待办或备忘后更新汇总数据用
    public enum UpdateIndexSummaryType
    {
        // 添加备忘录
        AddMemo,
        // 添加待办事项（未完成）
        AddToDo,
        // 添加待办事项（已完成）
        AddToDoCompleted,
        // 修改待办事项状态至已完成
        UpdateToDoCompleted
    }
}
