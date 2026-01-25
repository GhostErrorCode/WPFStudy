using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUiTest.Shared.Utilities
{
    // BCrypt加密验证工具类
    public static class PasswordHasher
    {
        // 默认工作因子，值越大越安全但越慢。10-12是常用平衡点。
        private const int DefaultWorkFactor = 11;


        /// <summary>
        /// 对明文密码进行哈希加密
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="workFactor">工作因子（可选）</param>
        /// <returns>哈希后的密码字符串</returns>
        public static string Hash(string password, int workFactor = DefaultWorkFactor)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("密码不能为空", nameof(password));
            }

            // BCrypt 会自动生成盐并包含在最终的结果字符串中
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        /// <summary>
        /// 验证明文密码是否与哈希密码匹配
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="hashedPassword">存储的哈希密码</param>
        /// <returns>匹配则返回 true</returns>
        public static bool Verify(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (BcryptAuthenticationException)
            {
                // 当哈希字符串格式无效等情况时
                return false;
            }
        }

        /// <summary>
        /// 检查给定的哈希字符串是否需要升级（例如，使用了旧的工作因子）
        /// </summary>
        /// <param name="hashedPassword">存储的哈希密码</param>
        /// <param name="newWorkFactor">新的工作因子</param>
        /// <returns>需要升级则返回 true</returns>
        public static bool NeedsUpgrade(string hashedPassword, int newWorkFactor = DefaultWorkFactor)
        {
            // BCrypt 的哈希字符串以 $2a$ 开头，并包含工作因子信息
            // 示例：$2a$11$r9CAYPQbKkO3fBqZ8L6k/eGN...
            // 解析出当前的工作因子进行比较
            if (hashedPassword.StartsWith("$2a$") && hashedPassword.Length > 4)
            {
                var parts = hashedPassword.Split('$');
                if (parts.Length >= 4 && int.TryParse(parts[2], out int currentWorkFactor))
                {
                    return currentWorkFactor < newWorkFactor;
                }
            }
            // 如果是非 BCrypt 哈希，也视为需要升级
            return !hashedPassword.StartsWith("$2a$");
        }
    }
}
