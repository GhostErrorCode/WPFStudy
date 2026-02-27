using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WpfUiTest.Shared.Models;

namespace WpfUiTest.Shared.Utilities
{
    // 登录凭证静态辅助Helper
    public static class LoginCredentialHelper
    {
        // 字段：登录凭证目录路径
        private static readonly string _loginCredentialDirectory = Path.Combine(AppContext.BaseDirectory,AppConfigurationHelper.LoadMergeSettings().LoginCredentialSettings.Directory);
        // 字段：登录凭证文件路径
        private static readonly string _loginCredentialFilePath = Path.Combine(_loginCredentialDirectory, AppConfigurationHelper.LoadMergeSettings().LoginCredentialSettings.FileName);

        // 静态方法：保存登录凭证
        public static async Task SaveLoginCredential(LoginCredential loginCredential)
        {
            try
            {
                // 设置凭证过期时间（优先读配置ExpireDays，默认30天，时间去除毫秒）
                loginCredential.Expires = DateTimeUtility.NowNoMilliseconds().AddDays(AppConfigurationHelper.LoadMergeSettings().LoginCredentialSettings.ExpireDays);
                // 凭证对象序列化为JSON字符串
                string json = JsonSerializer.Serialize(loginCredential);
                // JSON字符串转UTF8字节数组（加密/写文件需字节格式）
                byte[] data = Encoding.UTF8.GetBytes(json);
                // DPAPI加密字节数组（当前用户作用域，无附加熵）
                /*
                第 1 个参数data：要加密的原始字节数组（JSON 转换后的结果）。
                第 2 个参数null：加密的 “附加熵”（可选，用于提升加密安全性），此处设为null表示不使用附加熵。
                第 3 个参数DataProtectionScope.CurrentUser：加密作用域，指定为 “当前用户”（意味着只有当前登录 Windows 的用户能解密，其他用户无法解密）。
                */
                byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
                // 加密数据写入指定文件（覆盖已有文件）
                if (!File.Exists(_loginCredentialDirectory)) { Directory.CreateDirectory(_loginCredentialDirectory); }
                await File.WriteAllBytesAsync(_loginCredentialFilePath, encrypted);
            }
            catch(Exception ex)
            {
                throw;  // 异常信息抛给上层处理
            }
        }

        // 静态方法：读取登录凭证
        public static async Task<LoginCredential?> LoadLoginCredential()
        {
            // 检查文件是否存在，不存在则直接返回null
            if (!File.Exists(_loginCredentialFilePath)) return null;

            try
            {
                // 读取文件中的加密字节数组
                byte[] encrypted = await File.ReadAllBytesAsync(_loginCredentialFilePath);
                // 解密加密的字节数组
                byte[] data = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                // 将解密后的字节数组转换为UTF8编码的JSON字符串
                string json = Encoding.UTF8.GetString(data);
                // 将JSON字符串反序列化为LoginCredential类型对象并返回
                return JsonSerializer.Deserialize<LoginCredential>(json);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // 静态方法：删除登录凭证
        public static void DeleteLoginCredential()
        {
            // 如果登录凭证存在就删除它
            if (File.Exists(_loginCredentialFilePath)) { File.Delete(_loginCredentialFilePath); }
        }
    }
}
