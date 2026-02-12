using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Shared.Utilities
{
    /// <summary>
    /// 时间工具类
    /// 提供常用的时间格式化方法
    /// </summary>
    public static class DateTimeUtility
    {
        /// <summary>
        /// 获取当前时间（去掉毫秒部分）
        /// 数据库存储时更统一
        /// </summary>
        /// <returns>去掉毫秒的当前时间</returns>
        public static DateTime NowNoMilliseconds()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0);
        }

        /// <summary>
        /// 获取UTC当前时间（去掉毫秒部分）
        /// 推荐用于跨时区应用
        /// </summary>
        /// <returns>去掉毫秒的UTC当前时间</returns>
        public static DateTime UtcNowNoMilliseconds()
        {
            DateTime now = DateTime.UtcNow;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// 去掉时间的毫秒部分
        /// </summary>
        /// <param name="dateTime">原始时间</param>
        /// <returns>去掉毫秒的时间</returns>
        public static DateTime RemoveMilliseconds(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, dateTime.Second, 0);
        }

        /// <summary>
        /// 获取当前时间的格式化字符串
        /// </summary>
        /// <param name="format">
        /// 格式化字符串
        /// yyyy=年, MM=月, dd=日
        /// HH=时(24小时制), mm=分, ss=秒
        /// </param>
        /// <returns>格式化后的时间字符串</returns>
        public static string NowString(string format = "yyyy-MM-dd HH:mm:ss")
        {
            return DateTime.Now.ToString(format);
        }

        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <param name="dateTime">要格式化的时间</param>
        /// <param name="format">
        /// 格式化字符串
        /// yyyy=年, MM=月, dd=日
        /// HH=时(24小时制), mm=分, ss=秒
        /// </param>
        /// <returns>格式化后的时间字符串</returns>
        public static string Format(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }
    }
}
