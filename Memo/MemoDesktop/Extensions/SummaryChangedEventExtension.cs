using MemoDesktop.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Extensions
{
    // 首页汇总数据更改通知的扩展方法，用于订阅和发布事件
    public static class SummaryChangedEventExtension
    {
        // 发布首页汇总数据更新事件
        public static void SummaryChangedEventPublish(this IEventAggregator eventAggregator, SummaryChangedEventArgs summaryChangedEventArgs)
        {
            eventAggregator.GetEvent<SummaryChangedEvent>().Publish(summaryChangedEventArgs);
        }

        // 订阅首页汇总数据更新事件
        public static void SummaryChangedEventSubscribe(this IEventAggregator eventAggregator, Action<SummaryChangedEventArgs> summaryChangedEventArgsAction)
        {
            eventAggregator.GetEvent<SummaryChangedEvent>().Subscribe(summaryChangedEventArgsAction);
        }
    }
}
