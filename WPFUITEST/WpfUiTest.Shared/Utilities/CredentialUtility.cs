using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WpfUiTest.Shared.Models;

namespace WpfUiTest.Shared.Utilities
{
    // 登录凭证工具
    public class CredentialUtility
    {
        // 字段：配置文件
        private readonly IConfiguration _configuration;
        // 字段：ILogger日志管理
        private readonly ILogger<CredentialUtility> _logger;
        // 字段：完整的文件路径
        private readonly string _fullFilePath;

        public CredentialUtility(IConfiguration configuration, ILogger<CredentialUtility> logger)
        {
            // 初始化
            this._configuration = configuration;
            this._logger = logger;

            // 获取登录凭证目录并创建
            string filePath = Path.Combine(AppContext.BaseDirectory, configuration["Credential:Directory"] ?? "Cache");
            // 创建目录
            Directory.CreateDirectory(filePath);
            // 获取完整的文件路径与文件名
            this._fullFilePath = Path.Combine(filePath, configuration["Credential:FileName"] ?? "loginCache.dat");
        }

        // 保存加密凭证工具
        public void Save(LoginCredential credential)
        {
            try
            {
                // 设置凭证过期时间（优先读配置ExpireDays，默认30天，时间去除毫秒）
                credential.Expires = DateTimeUtility.NowNoMilliseconds().AddDays(double.TryParse(this._configuration["Credential:ExpireDays"] ?? "30", out double result) ? result : 30);
                // 凭证对象序列化为JSON字符串
                string json = JsonSerializer.Serialize(credential);
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
                File.WriteAllBytes(this._fullFilePath, encrypted);

                this._logger.LogInformation("登录凭证保存成功,有效期至{Expires}", credential.Expires);
            }
            catch(Exception ex)
            {
                this._logger.LogError("保存登录凭证时出现异常，异常信息:\n{ex}", ex);
            }
        }

        // 加载登录凭证
        public LoginCredential? Load()
        {
            // 检查文件是否存在，不存在则直接返回null
            if (!File.Exists(this._fullFilePath)) return null;

            try
            {
                // 读取文件中的加密字节数组
                byte[] encrypted = File.ReadAllBytes(this._fullFilePath);
                // 解密加密的字节数组
                byte[] data = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                // 将解密后的字节数组转换为UTF8编码的JSON字符串
                string json = Encoding.UTF8.GetString(data);
                // 将JSON字符串反序列化为LoginCredential类型对象并返回
                return JsonSerializer.Deserialize<LoginCredential>(json);
            }
            catch(Exception ex)
            {
                // 解密失败（被篡改、不同用户等）
                this._logger.LogError("登录凭证解密失败! 错误信息:\n{ex}", ex);
                return null;
            }
        }

        // 登出的时候删除已存在的登录凭证
        public void Delete()
        {
            // 如果登录凭证存在就删除它
            if (File.Exists(this._fullFilePath))
            {
                File.Delete(this._fullFilePath);
                this._logger.LogInformation("登录凭证已删除!");
            }
        }
    }
}
