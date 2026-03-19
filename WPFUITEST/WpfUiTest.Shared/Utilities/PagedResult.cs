using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace WpfUiTest.Shared.Utilities
{
    // 分页结果类
    public class PagedResult<T>
    {
        // 总条数
        public int TotalCount { get; set; } = 0;

        // 当前页数据列表
        public List<T> Items { get; set; } = new List<T>();
    }
}
