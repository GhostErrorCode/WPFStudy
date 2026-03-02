using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Shared.Extensions
{
    // 日期时间相关的扩展方法
    public static class DateTimeExtensions
    {
        // 根据当前的时间（小时）转为欢迎语
        public static string ToWelcomeMessage(this int hour)
        {
            return DateTime.Now.Hour switch
            {
                >= 0 and < 4 => "夜深了",
                >= 4 and < 8 => "早上好",
                >= 8 and < 12 => "上午好",
                >= 12 and < 14 => "中午好",
                >= 14 and < 18 => "下午好",
                >= 18 and < 24 => "晚上好",
                _ => "欢迎回来"
            };
        }
    }
}
