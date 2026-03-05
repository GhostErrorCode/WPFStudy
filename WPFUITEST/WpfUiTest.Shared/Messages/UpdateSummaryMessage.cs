using System;
using System.Collections.Generic;
using System.Text;
using WpfUiTest.Shared.Enums;

namespace WpfUiTest.Shared.Messages
{
    // 用于发送或接受的IMessenger参数，用于更新首页汇总数据
    public class UpdateIndexSummaryMessage
    {
        // 更新汇总数据类型，枚举类
        public UpdateIndexSummaryType Type { get; set; }
    }
}
