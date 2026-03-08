using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Core.DTOs.ToDo
{
    // 删除待办事项Dto
    public class DeleteToDoDto
    {
        // 待办事项ID
        public int Id { get; set; } = 0;

        // 用户ID
        public int UserId { get; set; } = 0;

        // 待办事项标题
        public string Title { get; set; } = string.Empty;
    }
}
