using System;
using System.Collections.Generic;
using System.Text;

namespace MemoDesktop.Events
{
    // 定义一个加载窗口事件，里面什么也不用写
    public class UpdateLoadingEvent : PubSubEvent<UpdateLoadingEventArgs>
    {
    }
}
