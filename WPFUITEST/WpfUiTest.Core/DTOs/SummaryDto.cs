using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Core.DTOs
{
    // 首页汇总数据传输对象
    public class SummaryDto
    {
        // 待办总数
        public int ToDoTotal { get; set; } = 0;
        // 待办完成数
        public int TodoCompleted { get; set; } = 0;
        // 待办完成率
        public string TodoCompletionRate { get; set; } = string.Empty;
        // 备忘录总数
        public int MemoTotal { get; set; } = 0;
        // 自定义
        public int Custom { get; set; } = 0;
    }
}
