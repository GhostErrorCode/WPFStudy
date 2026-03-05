using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Core.DTOs.Memo
{
    // 添加备忘录数据传输对象（用于添加备忘录）
    public class AddMemoDto
    {
        // 备忘录标题
        public string Title { get; set; } = string.Empty;

        // 备忘录内容
        public string Content { get; set; } = string.Empty;
    }
}
