using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Ui.Appearance;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Shared.Extensions
{
    // 应用主题扩展方法
    public static class ThemeExtensions
    {
        // 自定义枚举主题 转为 字符串主题
        public static string EnumThemeToStringTheme(this Theme theme)
        {
            return theme switch
            {
                Theme.Light => "明亮",
                Theme.Dark => "暗黑",
                Theme.HighContrast => "高对比度",
                _ => "未知主题"
            };
        }

        // 字符串主题 转为 自定义枚举主题
        public static Theme StringThemeToEnumTheme(this string str)
        {
            return str switch
            {
                "Light" => Theme.Light,
                "Dark" => Theme.Dark,
                "HighContrast" => Theme.HighContrast,
                _ => Theme.Light,
            };
        }

        // 字符串主题 转为 系统枚举主题
        public static ApplicationTheme StringThemeToApplicationTheme(this string str)
        {
            return str switch
            {
                "Light" => ApplicationTheme.Light,
                "Dark" => ApplicationTheme.Dark,
                "HighContrast" => ApplicationTheme.HighContrast,
                _ => ApplicationTheme.Light,
            };
        }

        // 自定义枚举主题 转为 系统枚举主题
        public static ApplicationTheme EnumThemeToApplicationTheme(this Theme theme)
        {
            return theme switch
            {
                Theme.Light => ApplicationTheme.Light,
                Theme.Dark => ApplicationTheme.Dark,
                Theme.HighContrast => ApplicationTheme.HighContrast,
                _ => ApplicationTheme.Light,
            };
        }
    }
}
