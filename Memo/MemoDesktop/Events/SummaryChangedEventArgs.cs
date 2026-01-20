using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Events
{
    // 首页汇总数据更新事件的参数类
    public class SummaryChangedEventArgs
    {
        // 待办事项总量
        public int ToDoTotalCount { get; set; } = 0;

        // 待办事项已完成数量
        public int ToDoCompletedCount { get; set; } = 0;

        // 待办事项完成比例
        public string ToDoCompletedProportion { get; set; } = string.Empty;

        // 备忘录总量
        public int MemoTotalCount { get; set; } = 0;
    }
}
