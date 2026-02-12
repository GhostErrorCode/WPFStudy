using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.DTOs.User
{
    // 数据传输对象 - 用户查询/返回结果
    public class UserResultDto
    {
        // 用户ID
        public int Id { get; set; }

        // 用户账户
        public string Account { get; set; } = "WpfUiTest";

        // 用户昵称
        public string UserName { get; set; } = "WpfUiTest";

        // 用户创建时间
        public DateTime CreateDate { get; set; }

        // 用户修改时间
        public DateTime UpdateDate { get; set; }
    }
}
