using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Utilities;

namespace WpfUiTest.Core.Data.Entities
{
    // 用户实体类
    public class User
    {
        // 用户ID
        public int Id { get; set; }

        // 用户账户
        public string Account { get; set; } = "WpfUiTest";

        // 用户昵称
        public string UserName { get; set; } = "WpfUiTest";

        // 用户密码
        public string Password { get; set; } = "123456";

        // 用户创建时间
        public DateTime CreateDate { get; set; } = DateTimeUtility.NowNoMilliseconds();

        // 用户修改时间
        public DateTime UpdateDate { get; set; } = DateTimeUtility.NowNoMilliseconds();

    }
}
