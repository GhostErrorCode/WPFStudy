using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Events
{
    // 消息提示器参数
    public class MessageEventArgs
    {
        // 过滤器（指定要显示在那个页面上）
        public string FilterName { get; set; } = string.Empty;

        // 消息
        public string Message { get; set; } = string.Empty;
    }
}
