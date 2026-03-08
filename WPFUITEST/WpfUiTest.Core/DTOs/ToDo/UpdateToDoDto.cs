using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Core.DTOs.ToDo
{
    // 修改待办事项Dto
    public class UpdateToDoDto
    {
        // 待办事项ID
        public int Id { get; set; } = 0;

        // 用户ID
        public int UserId { get; set; } = 0;

        // 待办事项标题
        public string Title { get; set; } = string.Empty;

        // 待办事项内容
        public string Content { get; set; } = string.Empty;

        // 待办事项状态
        public TodoStatusEnum Status { get; set; } = TodoStatusEnum.Pending;
    }
}
