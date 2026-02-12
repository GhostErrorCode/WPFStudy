using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Core.DTOs.User
{
    // 数据传输对象 - 用户登录
    public class LoginUserDto
    {
        // 用户账户
        public string Account { get; set; } = "WpfUiTest";

        // 用户密码
        public string Password { get; set; } = "123456";

        // 记住用户
        public bool IsRememberMe { get; set; } = false;
    }
}
