using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Core.DTOs.Memo
{
    // 分页查询备忘录条件数据传输对象
    public class PagedQueryMemoDto
    {
        // 当前页
        public int PageIndex { get; set; } = 1;

        // 页大小（每页显示的条数）
        public int PageSize { get; set; } = 20;

        // 备忘录标题
        public string Title { get; set; } = string.Empty;

        // 备忘录内容
        public string Content { get; set; } = string.Empty;
    }
}
