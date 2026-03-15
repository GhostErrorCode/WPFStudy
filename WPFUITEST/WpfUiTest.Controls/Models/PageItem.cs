using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Controls.Models
{
    /// <summary>
    /// 表示分页栏中的一个按钮项（数字页码或省略号）。
    /// 用于在 ItemsControl 中统一生成按钮。
    /// </summary>
    public class PageItem
    {
        /// <summary>按钮上显示的文字，例如 "1"、"..."</summary>
        public string DisplayText { get; set; } = string.Empty;

        /// <summary>
        /// 对应的页码。如果该项是省略号，则设为 -1，表示无实际页码。
        /// </summary>
        public int PageNumber { get; set; } = -1;

        /// <summary>按钮是否可点击（数字页码可点击，省略号不可点击）</summary>
        public bool IsEnabled { get; set; } = false;

        // 是否当前选中页
        public bool IsCurrent { get; set; } = false;
    }
}
