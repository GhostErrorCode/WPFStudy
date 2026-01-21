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
        public static void MessageEventSubscribe(this IEventAggregator eventAggregator, string filterName, Action<MessageEventArgs> action)
        {
            // 注册
            eventAggregator.GetEvent<MessageEvent>().Subscribe(
                // 参数1：事件处理委托
                action,

                // 参数2：线程选项 - 指定在哪个线程执行回调
                ThreadOption.PublisherThread,
                // PublisherThread: 在发布者所在的线程执行（默认，适合UI更新）
                // UIThread: 强制在UI线程执行
                // BackgroundThread: 在后台线程池执行

                // 参数3：保持订阅活跃
                true,
                // true: 即使订阅者被垃圾回收，订阅仍保持活跃（需注意内存泄漏）
                // false: 弱引用，当订阅者被回收时自动取消订阅

                // 参数4：过滤条件委托
                (MessageEventArgs messageEventArgs) => { return messageEventArgs.FilterName.Equals(filterName); }
                // 这是一个Lambda表达式，接收MessageEventArgs参数m
                // 只有当m.FilterName等于传入的filterName时，才触发action
                // 这样可以实现同一事件的不同类型消息分别处理
                );
        }

        // 发布消息提示
        public static void MessageEventPublish(this IEventAggregator eventAggregator, string filterName = "Main", string message = "")
        {
            // 发布
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageEventArgs() { FilterName = filterName, Message = message });
        }
    }
}
