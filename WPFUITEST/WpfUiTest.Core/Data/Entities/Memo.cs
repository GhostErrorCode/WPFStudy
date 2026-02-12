using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Data.Entities
{
    // 备忘录实体类
    public class Memo
    {
        // 备忘录ID
        public int Id { get; set; }

        // 用户ID
        public int UserId { get; set; }

        // 备忘录标题
        public string Title { get; set; } = string.Empty;

        // 备忘录内容
        public string Content { get; set; } = string.Empty;

        // 备忘录创建时间
        public DateTime CreateDate { get; set; } = DateTimeUtility.NowNoMilliseconds();

        // 备忘录修改时间
        public DateTime UpdateDate { get; set; } = DateTimeUtility.NowNoMilliseconds();
    }
}
