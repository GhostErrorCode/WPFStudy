using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Core.DTOs.ToDo
{
    // 分页查询待办事项条件数据传输对象
    public class PagedQueryToDoDto
    {
        // 当前页
        public int PageIndex { get; set; } = 1;

        // 页大小（每页显示的条数）
        public int PageSize { get; set; } = 20;

        // 待办事项标题
        public string Title { get; set; } = string.Empty;

        // 待办事项内容
        public string Content { get; set; } = string.Empty;

        // 待办事项状态（可空，这样可以传值null，用来获取全部待办事项）
        public TodoStatusEnum? Status { get; set; } = null;
    }
}
