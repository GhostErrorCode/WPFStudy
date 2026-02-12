using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Core.DTOs.User
{
    // 数据传输对象 - 注册用户
    public class RegisterUserDto
    {
        // 用户账户
        public string Account { get; set; } = "WpfUiTest";

        // 用户昵称
        public string UserName { get; set; } = "WpfUiTest";

        // 用户密码
        public string Password { get; set; } = "123456";

        // 用户确认密码
        public string ConfirmPassword { get; set; } = "123456";
        // 注意：不包含 Id、CreateDate、UpdateDate 字段
        // 因为这些字段应由服务器或数据库生成，不应由客户端提供
    }
}
