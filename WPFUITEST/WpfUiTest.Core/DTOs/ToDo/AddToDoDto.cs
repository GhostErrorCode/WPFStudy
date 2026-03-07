using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Core.DTOs.ToDo
{
    // 添加待办事项数据传输对象（用于添加待办事项）
    public class AddToDoDto
    {
        // 待办事项标题
        public string Title { get; set; } = string.Empty;

        // 待办事项内容
        public string Content { get; set; } = string.Empty;

        // 待办事项状态
        public TodoStatusEnum Status { get; set; } = TodoStatusEnum.Pending;
    }
}
