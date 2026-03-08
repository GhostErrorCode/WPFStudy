using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Core.DTOs.Memo
{
    // 删除备忘录Dto
    public class DeleteMemoDto
    {
        // 备忘录ID
        public int Id { get; set; } = 0;

        // 用户ID
        public int UserId { get; set; } = 0;

        // 备忘录标题
        public string Title { get; set; } = string.Empty;
    }
}
