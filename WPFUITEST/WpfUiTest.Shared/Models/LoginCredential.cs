using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Shared.Models
{
    /// <summary>保存在本地的自动登录凭证（加密后写入磁盘）</summary>
    public class LoginCredential
    {
        public int UserId { get; set; }
        public string Account { get; set; } = string.Empty;
        public DateTime Expires { get; set; }  // 过期时间
    }
}
