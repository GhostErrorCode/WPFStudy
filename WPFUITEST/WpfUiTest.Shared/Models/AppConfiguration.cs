using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;

namespace WpfUiTest.Shared.Models
{
    // 主要 - 应用程序设置Model
    public class AppConfiguration
    {
        // 应用程序设置
        public AppSettings AppSettings { get; set; } = new AppSettings();
        // 数据库设置
        public DataBaseSettings DataBaseSettings { get; set; } = new DataBaseSettings();
        // 日志设置
        public LogSettings LogSettings { get; set; } = new LogSettings();
        // 登录凭证设置
        public CredentialSettings CredentialSettings { get; set; } = new CredentialSettings();
        // 应用主题设置
        public ThemeSettings ThemeSettings { get; set; } = new ThemeSettings();
    }


    // =============== 子项 - JSON配置 ===============
    // 1.应用程序设置
    public class AppSettings
    {
        // 应用名称
        public string AppName { get; set; } = "GhostWpfUiTest";
        // 版本号
        public string Version { get; set; } = "alpha-v0.0.1";
        // 当前运行环境标识
        public string Environment { get; set; } = "Development";
    }

    // 2.数据库设置
    public class DataBaseSettings
    {
        // 数据库文件目录
        public string Directory { get; set; } = "DataBase";
        // 数据库连接串
        public string Connection { get; set; } = "DataBase/WpfUiTest.db;Cache=Shared";
    }

    // 3.日志设置
    public class LogSettings
    {
        // 日志文件目录
        public string Directory { get; set; } = "Logs";
        // 日志输出等级
        public LogLevel LogLevel { get; set; } = new LogLevel();
    }
    public class LogLevel
    {
        // 全局默认日志级别
        public string Default { get; set; } = "Information";

        // 微软组件日志级别
        public string Microsoft { get; set; } = "Warning";

        // System组件日志级别
        public string System { get; set; } = "Warning";
    }

    // 4.登录凭证设置
    public class CredentialSettings
    {
        // 凭证目录名
        public string Directory { get; set; } = "Cache";
        // 凭证文件名
        public string FileName { get; set; } = "loginCache.dat";
        // 过期时间
        public int ExpireDays { get; set; } = 30;
    }

    // 5.应用主题设置
    public class ThemeSettings
    {
        // 应用主题
        public string Theme { get; set; } = "Light";
    }
}
