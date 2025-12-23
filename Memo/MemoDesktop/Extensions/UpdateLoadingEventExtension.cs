using MemoDesktop.Events;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MemoDesktop.Extensions
{
    // 加载动画的扩展方法，用于订阅和发布事件
    public static class UpdateLoadingEventExtension
    {
        // 发布加载动画事件
        public static void UpdateLoadingEventPublish(this IEventAggregator eventAggregator, UpdateLoadingEventArgs updateLoadingEventArgs)
        {
            // 通过事件聚合器获取到UpdateLoadingEvent事件，并发布
            eventAggregator.GetEvent<UpdateLoadingEvent>().Publish(updateLoadingEventArgs);
        }

        // 订阅加载动画事件
        public static void UpdateLoadingEventSubscribe(this IEventAggregator eventAggregator, Action<UpdateLoadingEventArgs> updateLoadingEventArgsAction)
        {
            eventAggregator.GetEvent<UpdateLoadingEvent>().Subscribe(updateLoadingEventArgsAction);
        }
    }
}
