using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Events
{
    // 首页汇总数据更新事件
    public class SummaryChangedEvent : PubSubEvent<SummaryChangedEventArgs>
    {
    }
}
