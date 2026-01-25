using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Data.Entities
{
    // 备忘录实体类
    public class ToDo
    {
        // 待办事项ID
        public int Id { get; set; }

        // 用户ID
        public int UserId { get; set; }

        // 待办事项标题
        public string Title { get; set; } = string.Empty;

        // 待办事项内容
        public string Content { get; set; } = string.Empty;

        // 待办事项状态
        public TodoStatusEnum Status { get; set; } = TodoStatusEnum.Pending;

        // 待办事项创建时间
        public DateTime CreateDate { get; set; } = DateTimeUtility.NowNoMilliseconds();

        // 待办事项修改时间
        public DateTime UpdateDate { get; set; } = DateTimeUtility.NowNoMilliseconds();

    }
}
