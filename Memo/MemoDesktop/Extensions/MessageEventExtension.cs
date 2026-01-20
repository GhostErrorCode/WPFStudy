using MemoDesktop.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Extensions
{
    // 消息提示框的扩展方法
    public static class MessageEventExtension
    {
        // 注册消息提示
        public static void MessageEventSubscribe(this IEventAggregator eventAggregator, Action<string> action)
        {
            // 注册
            eventAggregator.GetEvent<MessageEvent>().Subscribe(action);
        }

        // 发布消息提示
        public static void MessageEventPublish(this IEventAggregator eventAggregator, string message)
        {
            // 发布
            eventAggregator.GetEvent<MessageEvent>().Publish(message);
        }
    }
}
