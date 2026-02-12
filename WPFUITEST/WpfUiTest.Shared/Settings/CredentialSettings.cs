using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Shared.Settings
{
    /// <summary>自动登录凭证的配置项（对应 appsettings.json 的 Credential 节点）</summary>
    public class CredentialSettings
    {
        public string FileName { get; set; } = "login.dat";  // 凭证文件名
        public string Directory { get; set; } = "Cache";  // 存储目录
        public int ExpireDays { get; set; } = 30;  // 有效期（天）
    }
}
