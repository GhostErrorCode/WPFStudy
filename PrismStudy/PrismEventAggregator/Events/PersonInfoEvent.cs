using PrismEventAggregator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrismEventAggregator.Events
{
    /// <summary>
    /// 人员信息事件类
    /// 继承自PubSubEvent<PersonInfo>，用于在应用程序中传递人员信息
    /// </summary>
    public class PersonInfoEvent : PubSubEvent<PersonInfo>
    {
        // 这个类不需要额外实现，继承PubSubEvent就具备了发布订阅功能
    }
}
