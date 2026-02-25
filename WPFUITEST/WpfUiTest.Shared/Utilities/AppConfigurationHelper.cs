using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using WpfUiTest.Shared.Models;

namespace WpfUiTest.Shared.Utilities
{
    // 应用程序设置静态辅助Helper
    public static class AppConfigurationHelper
    {
        // 字段：配置文件存放目录
        private static readonly string _appSettingsDirectory = Path.Combine(AppContext.BaseDirectory, "Settings");
        // 字段：JsonSerializerOptions Json序列化选项
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true, // 忽略JSON和C#属性的大小写差异
            WriteIndented = true, // 序列化时格式化JSON（仅影响保存，读取无影响）
            ReadCommentHandling = JsonCommentHandling.Skip, // 忽略注释
            AllowTrailingCommas = true // 可选：允许尾随逗号，增强容错
        };

        // 静态方法 - 加载并合并用户设置
        public static AppConfiguration LoadMergeSettings()
        {
            // 1.读取默认设置
            AppConfiguration appConfiguration = LoadSettings<AppConfiguration>("AppSettings.json");
            // 2.读取用户设置
            AppConfiguration userAppConfiguration = LoadSettings<AppConfiguration>("UserAppSettings.json");
            // 3.合并用户设置
            MergeSettings(appConfiguration, userAppConfiguration);
            // 4.返回合并后的最终设置
            return appConfiguration;
        }

        // 静态方法 - 保存用户设置
        public static void SaveUserSettings(AppConfiguration settings)
        {
            // 确保配置目录存在
            Directory.CreateDirectory(_appSettingsDirectory);
            // 拼接完整的用户 JSON 设置文件路径
            string SettingsPath = Path.Combine(_appSettingsDirectory, "UserAppSettings.json");

            // 序列化并保存JSON（格式化，便于手动修改）
            string jsonContent = JsonSerializer.Serialize(settings, _jsonSerializerOptions);
            // 写入用户配置JSON文件中
            File.WriteAllText(SettingsPath, jsonContent, Encoding.UTF8);
        }


        // ========== 内部方法 ==========
        // 私有静态方法 - 加载设置(T必须有无参数的构造函数)
        private static T LoadSettings<T>(string fileName) where T : new()
        {
            try
            {
                // 确保配置目录存在
                Directory.CreateDirectory(_appSettingsDirectory);
                // 拼接完整的 JSON 设置文件路径
                string SettingsPath = Path.Combine(_appSettingsDirectory, fileName);

                // 如果文件存在，则读取 JSON 内容并反序列化为实例
                // 如果文件不存在，则返回默认实例，使用实例中的默认值
                if (File.Exists(SettingsPath))
                {
                    // 文件存在
                    // 读取JSON内容并反序列化（忽略大小写，匹配更灵活）
                    string jsonContent = File.ReadAllText(SettingsPath, Encoding.UTF8);
                    return JsonSerializer.Deserialize<T>(jsonContent, _jsonSerializerOptions) ?? new T();  // 反序列化失败则返回默认实例
                }
                else
                {
                    // 文件不存在
                    return new T();
                }
            }
            catch (Exception ex)
            {
                // 将异常抛给上层处理
                throw;
            }
        }
        // 私有静态方法 - 加载默认配置后合并用户自定义配置
        private static void MergeSettings(AppConfiguration baseSettings, AppConfiguration userSettings)
        {
            // 1. 合并应用基础设置
            if (!string.IsNullOrEmpty(userSettings.AppSettings.AppName))
                baseSettings.AppSettings.AppName = userSettings.AppSettings.AppName;
            if (!string.IsNullOrEmpty(userSettings.AppSettings.Version))
                baseSettings.AppSettings.Version = userSettings.AppSettings.Version;
            if (!string.IsNullOrEmpty(userSettings.AppSettings.Environment))
                baseSettings.AppSettings.Environment = userSettings.AppSettings.Environment;

            // 2. 合并数据库设置
            if (!string.IsNullOrEmpty(userSettings.DataBaseSettings.Directory))
                baseSettings.DataBaseSettings.Directory = userSettings.DataBaseSettings.Directory;
            if (!string.IsNullOrEmpty(userSettings.DataBaseSettings.Connection))
                baseSettings.DataBaseSettings.Connection = userSettings.DataBaseSettings.Connection;

            // 3. 合并日志设置
            if (!string.IsNullOrEmpty(userSettings.LogSettings.Directory))
                baseSettings.LogSettings.Directory = userSettings.LogSettings.Directory;
            // 合并日志级别
            if (!string.IsNullOrEmpty(userSettings.LogSettings.LogLevel.Default))
                baseSettings.LogSettings.LogLevel.Default = userSettings.LogSettings.LogLevel.Default;
            if (!string.IsNullOrEmpty(userSettings.LogSettings.LogLevel.Microsoft))
                baseSettings.LogSettings.LogLevel.Microsoft = userSettings.LogSettings.LogLevel.Microsoft;
            if (!string.IsNullOrEmpty(userSettings.LogSettings.LogLevel.System))
                baseSettings.LogSettings.LogLevel.System = userSettings.LogSettings.LogLevel.System;

            // 4. 合并登录凭证设置
            if (!string.IsNullOrEmpty(userSettings.CredentialSettings.Directory))
                baseSettings.CredentialSettings.Directory = userSettings.CredentialSettings.Directory;
            if (!string.IsNullOrEmpty(userSettings.CredentialSettings.FileName))
                baseSettings.CredentialSettings.FileName = userSettings.CredentialSettings.FileName;
            if (userSettings.CredentialSettings.ExpireDays > 0) // 过期天数>0才覆盖（避免0覆盖默认30）
                baseSettings.CredentialSettings.ExpireDays = userSettings.CredentialSettings.ExpireDays;

            // 5. 合并主题设置
            if (!string.IsNullOrEmpty(userSettings.ThemeSettings.Theme))
                baseSettings.ThemeSettings.Theme = userSettings.ThemeSettings.Theme;
        }

    }
}
